using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mogre;
using System.Drawing;
using Mogre.Helpers;

namespace MogreTest
{
    public class OgreWindow
    {
        public Root root;
        public SceneManager sceneMgr;

        protected Camera camera;
        protected Viewport viewport;
        protected RenderWindow window;
        protected Point position;
        protected IntPtr hWnd;

        public OgreWindow(Point origin, IntPtr hWnd)
        {
            position = origin;
            this.hWnd = hWnd;
        }

        public void InitMogre()
        {

            //----------------------------------------------------- 
            // 1 enter ogre 
            //----------------------------------------------------- 
            root = new Root();

          
            ResourceGroupManager.Singleton.AddResourceLocation( "./media","FileSystem",ResourceGroupManager.DEFAULT_RESOURCE_GROUP_NAME );
            //----------------------------------------------------- 
            // 3 Configures the application and creates the window
            //----------------------------------------------------- 
            bool foundit = false;
            foreach (RenderSystem rs in root.GetAvailableRenderers())
            {
                root.RenderSystem = rs;
                String rname = root.RenderSystem.Name;
                if (rname == "Direct3D9 Rendering Subsystem")
                {
                    foundit = true;
                    break;
                }
            }

            if (!foundit)
                return; //we didn't find it... Raise exception?

            //we found it, we might as well use it!
            root.RenderSystem.SetConfigOption("Full Screen", "No");
            root.RenderSystem.SetConfigOption("Video Mode", "640 x 480 @ 32-bit colour");

            root.Initialise(false);
            NameValuePairList misc = new NameValuePairList();
            misc["externalWindowHandle"] = hWnd.ToString();
            window = root.CreateRenderWindow("Simple Mogre Form Window", 0, 0, false, misc);
            ResourceGroupManager.Singleton.InitialiseAllResourceGroups();

            //----------------------------------------------------- 
            // 4 Create the SceneManager
            // 
            //		ST_GENERIC = octree
            //		ST_EXTERIOR_CLOSE = simple terrain
            //		ST_EXTERIOR_FAR = nature terrain (depreciated)
            //		ST_EXTERIOR_REAL_FAR = paging landscape
            //		ST_INTERIOR = Quake3 BSP
            //----------------------------------------------------- 
            sceneMgr = root.CreateSceneManager(SceneType.ST_GENERIC, "SceneMgr");
            sceneMgr.AmbientLight = new ColourValue(0.5f, 0.5f, 0.5f);

            // Create a light
            Light l = sceneMgr.CreateLight("MainLight");
            // Accept default settings: point light, white diffuse, just set position
            // NB I could attach the light to a SceneNode if I wanted it to move automatically with
            //  other objects, but I don't
            l.Position = new Vector3(20, 80, 50);

            //----------------------------------------------------- 
            // 5 Create the camera 
            //----------------------------------------------------- 
            camera = sceneMgr.CreateCamera("SimpleCamera");
            camera.Position = new Vector3(0f, 0f, 100f);
            // Look back along -Z
            camera.LookAt(new Vector3(0f, 0f, -300f));
            camera.NearClipDistance = 5;
           

            viewport = window.AddViewport(camera);
            viewport.BackgroundColour = new ColourValue(0.0f, 0.0f, 0.0f, 1.0f);


           /* Entity ent = sceneMgr.CreateEntity("ogre", "ogrehead.mesh");
            SceneNode node = sceneMgr.RootSceneNode.CreateChildSceneNode("ogreNode");
            node.AttachObject(ent);*/

            MaterialPtr material = MaterialManager.Singleton.Create("Test/ColourTest",
                                             ResourceGroupManager.DEFAULT_RESOURCE_GROUP_NAME);
            material.GetTechnique(0).GetPass(0).VertexColourTracking =
                                                              (int)TrackVertexColourEnum.TVC_AMBIENT;

        }

        public  void CreateMesh(string name, Point3D[] Vertexs,int[] TriangleIndices)
        {
            MeshBuilderHelper mbh = new MeshBuilderHelper(name, "General", false  , 0,(uint) Vertexs.Length);


            UInt32 offPos = mbh.AddElement(VertexElementType.VET_FLOAT3,
                                           VertexElementSemantic.VES_POSITION).Offset;
            UInt32 offNorm = mbh.AddElement(VertexElementType.VET_FLOAT3,
                                            VertexElementSemantic.VES_NORMAL).Offset;

            UInt32 offDiff = mbh.AddElement(VertexElementType.VET_FLOAT3,
                                            VertexElementSemantic.VES_DIFFUSE).Offset;

            UInt32 offTex = mbh.AddElement(VertexElementType.VET_FLOAT3,
                                            VertexElementSemantic.VES_TEXTURE_COORDINATES ).Offset;
 

            mbh.CreateVertexBuffer((uint)Vertexs.Length  , HardwareBuffer.Usage.HBU_STATIC_WRITE_ONLY);
            Point3D cross ;
            for (uint i = 0; i < Vertexs.Length; i++)
            {
                mbh.SetVertFloat(i, offPos,(float) Vertexs[i].X , (float)Vertexs[i].Y ,(float) Vertexs[i].Z );      //position
                cross= Point3D.Normalize(Vertexs[i]);
                mbh.SetVertFloat(i,offNorm ,(float)cross.X,(float)cross.Y ,(float)cross.Z  );
                mbh.SetVertFloat(i, offDiff, 1,1, 1);      //color

                double angle=System.Math.Atan2( Vertexs[i].X , Vertexs[i].Y )/2/System.Math.PI ;
                if (angle<0) angle =1 + angle ;

                mbh.SetVertFloat(i,offTex,(float)angle,(float)(Vertexs[i].Z/25d) );
            }

            uint  TriangleCount =(uint)((double) TriangleIndices.Length / 4d);


            mbh.CreateIndexBuffer(TriangleCount, HardwareIndexBuffer.IndexType.IT_16BIT,
                                  HardwareBuffer.Usage.HBU_STATIC_WRITE_ONLY);

            uint cTriangle = 0;
            for (uint i = 0; i < TriangleIndices.Length; i += 4)
            {
                mbh.SetIndex16bit(cTriangle , (UInt16)TriangleIndices[i], (UInt16)TriangleIndices[i+1], (UInt16)TriangleIndices[i+2]);
                cTriangle++;
            }


            MeshPtr m = mbh.SetMaterial("Examples/TransparentTest");//SetMaterial("Test/ColourTest");
            m._setBounds(new AxisAlignedBox(-20.0f, -20.0f,-20.0f, 20.0f, 20.0f, 20.0f), false);
            m._setBoundingSphereRadius((float)System.Math.Sqrt(10.0f * 10.0f + 10.0f * 10.0f));


            Entity ent1 = sceneMgr.CreateEntity("MarchingMesh" + name , name );
            ent1.CastShadows = true;
          
            SceneNode  node1 = sceneMgr.RootSceneNode.CreateChildSceneNode("MarchingMeshNode"+name );
            node1.AttachObject(ent1);
            nodes.Add(node1);
        }

        List<SceneNode> nodes=new List<SceneNode>(); 

        public void Paint()
        {
            for (int i = 0; i < nodes.Count; i++)
            {
                if (i%2==0)
                    nodes[i].Yaw((Mogre.Radian)(.1), Node.TransformSpace.TS_WORLD);
                else
                    nodes[i].Yaw((Mogre.Radian)(-.1), Node.TransformSpace.TS_WORLD);
            }
            root.RenderOneFrame();
        }

        public void Dispose()
        {
            if (root != null)
            {
                root.Dispose();
                root = null;
            }
        }
    }
}
