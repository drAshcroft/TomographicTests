using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;


/*
   Given a grid cell and an isolevel, calculate the triangular
   facets required to represent the isosurface through the cell.
   Return the number of triangular facets, the array "triangles"
   will be loaded up with the vertices at most 5 triangular facets.
    0 will be returned if the grid cell is either totally above
   of totally below the isolevel.
*/
public class MarchingCubes
{


    #region StructDefinitions

    private struct TRIANGLE
    {
        public Point3D[] p;
        public TRIANGLE(int nPoints)
        {
            p = new Point3D[3];
        }
    }

    private struct GRIDCELL
    {
        public Point3D[] p;
        public double[] val;
        public GRIDCELL(int nPoints)
        {
            p = new Point3D[8];
            val = new double[8];
        }
    }
    #endregion
    #region edgeTable
    int[] edgeTable = new int[]{
0x0  , 0x109, 0x203, 0x30a, 0x406, 0x50f, 0x605, 0x70c,
0x80c, 0x905, 0xa0f, 0xb06, 0xc0a, 0xd03, 0xe09, 0xf00,
0x190, 0x99 , 0x393, 0x29a, 0x596, 0x49f, 0x795, 0x69c,
0x99c, 0x895, 0xb9f, 0xa96, 0xd9a, 0xc93, 0xf99, 0xe90,
0x230, 0x339, 0x33 , 0x13a, 0x636, 0x73f, 0x435, 0x53c,
0xa3c, 0xb35, 0x83f, 0x936, 0xe3a, 0xf33, 0xc39, 0xd30,
0x3a0, 0x2a9, 0x1a3, 0xaa , 0x7a6, 0x6af, 0x5a5, 0x4ac,
0xbac, 0xaa5, 0x9af, 0x8a6, 0xfaa, 0xea3, 0xda9, 0xca0,
0x460, 0x569, 0x663, 0x76a, 0x66 , 0x16f, 0x265, 0x36c,
0xc6c, 0xd65, 0xe6f, 0xf66, 0x86a, 0x963, 0xa69, 0xb60,
0x5f0, 0x4f9, 0x7f3, 0x6fa, 0x1f6, 0xff , 0x3f5, 0x2fc,
0xdfc, 0xcf5, 0xfff, 0xef6, 0x9fa, 0x8f3, 0xbf9, 0xaf0,
0x650, 0x759, 0x453, 0x55a, 0x256, 0x35f, 0x55 , 0x15c,
0xe5c, 0xf55, 0xc5f, 0xd56, 0xa5a, 0xb53, 0x859, 0x950,
0x7c0, 0x6c9, 0x5c3, 0x4ca, 0x3c6, 0x2cf, 0x1c5, 0xcc ,
0xfcc, 0xec5, 0xdcf, 0xcc6, 0xbca, 0xac3, 0x9c9, 0x8c0,
0x8c0, 0x9c9, 0xac3, 0xbca, 0xcc6, 0xdcf, 0xec5, 0xfcc,
0xcc , 0x1c5, 0x2cf, 0x3c6, 0x4ca, 0x5c3, 0x6c9, 0x7c0,
0x950, 0x859, 0xb53, 0xa5a, 0xd56, 0xc5f, 0xf55, 0xe5c,
0x15c, 0x55 , 0x35f, 0x256, 0x55a, 0x453, 0x759, 0x650,
0xaf0, 0xbf9, 0x8f3, 0x9fa, 0xef6, 0xfff, 0xcf5, 0xdfc,
0x2fc, 0x3f5, 0xff , 0x1f6, 0x6fa, 0x7f3, 0x4f9, 0x5f0,
0xb60, 0xa69, 0x963, 0x86a, 0xf66, 0xe6f, 0xd65, 0xc6c,
0x36c, 0x265, 0x16f, 0x66 , 0x76a, 0x663, 0x569, 0x460,
0xca0, 0xda9, 0xea3, 0xfaa, 0x8a6, 0x9af, 0xaa5, 0xbac,
0x4ac, 0x5a5, 0x6af, 0x7a6, 0xaa , 0x1a3, 0x2a9, 0x3a0,
0xd30, 0xc39, 0xf33, 0xe3a, 0x936, 0x83f, 0xb35, 0xa3c,
0x53c, 0x435, 0x73f, 0x636, 0x13a, 0x33 , 0x339, 0x230,
0xe90, 0xf99, 0xc93, 0xd9a, 0xa96, 0xb9f, 0x895, 0x99c,
0x69c, 0x795, 0x49f, 0x596, 0x29a, 0x393, 0x99 , 0x190,
0xf00, 0xe09, 0xd03, 0xc0a, 0xb06, 0xa0f, 0x905, 0x80c,
0x70c, 0x605, 0x50f, 0x406, 0x30a, 0x203, 0x109, 0x0   };
    #endregion
    #region triTable
    int[,] triTable = new int[,]{{-1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
{0, 8, 3, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
{0, 1, 9, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
{1, 8, 3, 9, 8, 1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
{1, 2, 10, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
{0, 8, 3, 1, 2, 10, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
{9, 2, 10, 0, 2, 9, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
{2, 8, 3, 2, 10, 8, 10, 9, 8, -1, -1, -1, -1, -1, -1, -1},
{3, 11, 2, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
{0, 11, 2, 8, 11, 0, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
{1, 9, 0, 2, 3, 11, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
{1, 11, 2, 1, 9, 11, 9, 8, 11, -1, -1, -1, -1, -1, -1, -1},
{3, 10, 1, 11, 10, 3, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
{0, 10, 1, 0, 8, 10, 8, 11, 10, -1, -1, -1, -1, -1, -1, -1},
{3, 9, 0, 3, 11, 9, 11, 10, 9, -1, -1, -1, -1, -1, -1, -1},
{9, 8, 10, 10, 8, 11, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
{4, 7, 8, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
{4, 3, 0, 7, 3, 4, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
{0, 1, 9, 8, 4, 7, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
{4, 1, 9, 4, 7, 1, 7, 3, 1, -1, -1, -1, -1, -1, -1, -1},
{1, 2, 10, 8, 4, 7, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
{3, 4, 7, 3, 0, 4, 1, 2, 10, -1, -1, -1, -1, -1, -1, -1},
{9, 2, 10, 9, 0, 2, 8, 4, 7, -1, -1, -1, -1, -1, -1, -1},
{2, 10, 9, 2, 9, 7, 2, 7, 3, 7, 9, 4, -1, -1, -1, -1},
{8, 4, 7, 3, 11, 2, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
{11, 4, 7, 11, 2, 4, 2, 0, 4, -1, -1, -1, -1, -1, -1, -1},
{9, 0, 1, 8, 4, 7, 2, 3, 11, -1, -1, -1, -1, -1, -1, -1},
{4, 7, 11, 9, 4, 11, 9, 11, 2, 9, 2, 1, -1, -1, -1, -1},
{3, 10, 1, 3, 11, 10, 7, 8, 4, -1, -1, -1, -1, -1, -1, -1},
{1, 11, 10, 1, 4, 11, 1, 0, 4, 7, 11, 4, -1, -1, -1, -1},
{4, 7, 8, 9, 0, 11, 9, 11, 10, 11, 0, 3, -1, -1, -1, -1},
{4, 7, 11, 4, 11, 9, 9, 11, 10, -1, -1, -1, -1, -1, -1, -1},
{9, 5, 4, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
{9, 5, 4, 0, 8, 3, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
{0, 5, 4, 1, 5, 0, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
{8, 5, 4, 8, 3, 5, 3, 1, 5, -1, -1, -1, -1, -1, -1, -1},
{1, 2, 10, 9, 5, 4, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
{3, 0, 8, 1, 2, 10, 4, 9, 5, -1, -1, -1, -1, -1, -1, -1},
{5, 2, 10, 5, 4, 2, 4, 0, 2, -1, -1, -1, -1, -1, -1, -1},
{2, 10, 5, 3, 2, 5, 3, 5, 4, 3, 4, 8, -1, -1, -1, -1},
{9, 5, 4, 2, 3, 11, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
{0, 11, 2, 0, 8, 11, 4, 9, 5, -1, -1, -1, -1, -1, -1, -1},
{0, 5, 4, 0, 1, 5, 2, 3, 11, -1, -1, -1, -1, -1, -1, -1},
{2, 1, 5, 2, 5, 8, 2, 8, 11, 4, 8, 5, -1, -1, -1, -1},
{10, 3, 11, 10, 1, 3, 9, 5, 4, -1, -1, -1, -1, -1, -1, -1},
{4, 9, 5, 0, 8, 1, 8, 10, 1, 8, 11, 10, -1, -1, -1, -1},
{5, 4, 0, 5, 0, 11, 5, 11, 10, 11, 0, 3, -1, -1, -1, -1},
{5, 4, 8, 5, 8, 10, 10, 8, 11, -1, -1, -1, -1, -1, -1, -1},
{9, 7, 8, 5, 7, 9, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
{9, 3, 0, 9, 5, 3, 5, 7, 3, -1, -1, -1, -1, -1, -1, -1},
{0, 7, 8, 0, 1, 7, 1, 5, 7, -1, -1, -1, -1, -1, -1, -1},
{1, 5, 3, 3, 5, 7, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
{9, 7, 8, 9, 5, 7, 10, 1, 2, -1, -1, -1, -1, -1, -1, -1},
{10, 1, 2, 9, 5, 0, 5, 3, 0, 5, 7, 3, -1, -1, -1, -1},
{8, 0, 2, 8, 2, 5, 8, 5, 7, 10, 5, 2, -1, -1, -1, -1},
{2, 10, 5, 2, 5, 3, 3, 5, 7, -1, -1, -1, -1, -1, -1, -1},
{7, 9, 5, 7, 8, 9, 3, 11, 2, -1, -1, -1, -1, -1, -1, -1},
{9, 5, 7, 9, 7, 2, 9, 2, 0, 2, 7, 11, -1, -1, -1, -1},
{2, 3, 11, 0, 1, 8, 1, 7, 8, 1, 5, 7, -1, -1, -1, -1},
{11, 2, 1, 11, 1, 7, 7, 1, 5, -1, -1, -1, -1, -1, -1, -1},
{9, 5, 8, 8, 5, 7, 10, 1, 3, 10, 3, 11, -1, -1, -1, -1},
{5, 7, 0, 5, 0, 9, 7, 11, 0, 1, 0, 10, 11, 10, 0, -1},
{11, 10, 0, 11, 0, 3, 10, 5, 0, 8, 0, 7, 5, 7, 0, -1},
{11, 10, 5, 7, 11, 5, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
{10, 6, 5, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
{0, 8, 3, 5, 10, 6, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
{9, 0, 1, 5, 10, 6, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
{1, 8, 3, 1, 9, 8, 5, 10, 6, -1, -1, -1, -1, -1, -1, -1},
{1, 6, 5, 2, 6, 1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
{1, 6, 5, 1, 2, 6, 3, 0, 8, -1, -1, -1, -1, -1, -1, -1},
{9, 6, 5, 9, 0, 6, 0, 2, 6, -1, -1, -1, -1, -1, -1, -1},
{5, 9, 8, 5, 8, 2, 5, 2, 6, 3, 2, 8, -1, -1, -1, -1},
{2, 3, 11, 10, 6, 5, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
{11, 0, 8, 11, 2, 0, 10, 6, 5, -1, -1, -1, -1, -1, -1, -1},
{0, 1, 9, 2, 3, 11, 5, 10, 6, -1, -1, -1, -1, -1, -1, -1},
{5, 10, 6, 1, 9, 2, 9, 11, 2, 9, 8, 11, -1, -1, -1, -1},
{6, 3, 11, 6, 5, 3, 5, 1, 3, -1, -1, -1, -1, -1, -1, -1},
{0, 8, 11, 0, 11, 5, 0, 5, 1, 5, 11, 6, -1, -1, -1, -1},
{3, 11, 6, 0, 3, 6, 0, 6, 5, 0, 5, 9, -1, -1, -1, -1},
{6, 5, 9, 6, 9, 11, 11, 9, 8, -1, -1, -1, -1, -1, -1, -1},
{5, 10, 6, 4, 7, 8, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
{4, 3, 0, 4, 7, 3, 6, 5, 10, -1, -1, -1, -1, -1, -1, -1},
{1, 9, 0, 5, 10, 6, 8, 4, 7, -1, -1, -1, -1, -1, -1, -1},
{10, 6, 5, 1, 9, 7, 1, 7, 3, 7, 9, 4, -1, -1, -1, -1},
{6, 1, 2, 6, 5, 1, 4, 7, 8, -1, -1, -1, -1, -1, -1, -1},
{1, 2, 5, 5, 2, 6, 3, 0, 4, 3, 4, 7, -1, -1, -1, -1},
{8, 4, 7, 9, 0, 5, 0, 6, 5, 0, 2, 6, -1, -1, -1, -1},
{7, 3, 9, 7, 9, 4, 3, 2, 9, 5, 9, 6, 2, 6, 9, -1},
{3, 11, 2, 7, 8, 4, 10, 6, 5, -1, -1, -1, -1, -1, -1, -1},
{5, 10, 6, 4, 7, 2, 4, 2, 0, 2, 7, 11, -1, -1, -1, -1},
{0, 1, 9, 4, 7, 8, 2, 3, 11, 5, 10, 6, -1, -1, -1, -1},
{9, 2, 1, 9, 11, 2, 9, 4, 11, 7, 11, 4, 5, 10, 6, -1},
{8, 4, 7, 3, 11, 5, 3, 5, 1, 5, 11, 6, -1, -1, -1, -1},
{5, 1, 11, 5, 11, 6, 1, 0, 11, 7, 11, 4, 0, 4, 11, -1},
{0, 5, 9, 0, 6, 5, 0, 3, 6, 11, 6, 3, 8, 4, 7, -1},
{6, 5, 9, 6, 9, 11, 4, 7, 9, 7, 11, 9, -1, -1, -1, -1},
{10, 4, 9, 6, 4, 10, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
{4, 10, 6, 4, 9, 10, 0, 8, 3, -1, -1, -1, -1, -1, -1, -1},
{10, 0, 1, 10, 6, 0, 6, 4, 0, -1, -1, -1, -1, -1, -1, -1},
{8, 3, 1, 8, 1, 6, 8, 6, 4, 6, 1, 10, -1, -1, -1, -1},
{1, 4, 9, 1, 2, 4, 2, 6, 4, -1, -1, -1, -1, -1, -1, -1},
{3, 0, 8, 1, 2, 9, 2, 4, 9, 2, 6, 4, -1, -1, -1, -1},
{0, 2, 4, 4, 2, 6, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
{8, 3, 2, 8, 2, 4, 4, 2, 6, -1, -1, -1, -1, -1, -1, -1},
{10, 4, 9, 10, 6, 4, 11, 2, 3, -1, -1, -1, -1, -1, -1, -1},
{0, 8, 2, 2, 8, 11, 4, 9, 10, 4, 10, 6, -1, -1, -1, -1},
{3, 11, 2, 0, 1, 6, 0, 6, 4, 6, 1, 10, -1, -1, -1, -1},
{6, 4, 1, 6, 1, 10, 4, 8, 1, 2, 1, 11, 8, 11, 1, -1},
{9, 6, 4, 9, 3, 6, 9, 1, 3, 11, 6, 3, -1, -1, -1, -1},
{8, 11, 1, 8, 1, 0, 11, 6, 1, 9, 1, 4, 6, 4, 1, -1},
{3, 11, 6, 3, 6, 0, 0, 6, 4, -1, -1, -1, -1, -1, -1, -1},
{6, 4, 8, 11, 6, 8, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
{7, 10, 6, 7, 8, 10, 8, 9, 10, -1, -1, -1, -1, -1, -1, -1},
{0, 7, 3, 0, 10, 7, 0, 9, 10, 6, 7, 10, -1, -1, -1, -1},
{10, 6, 7, 1, 10, 7, 1, 7, 8, 1, 8, 0, -1, -1, -1, -1},
{10, 6, 7, 10, 7, 1, 1, 7, 3, -1, -1, -1, -1, -1, -1, -1},
{1, 2, 6, 1, 6, 8, 1, 8, 9, 8, 6, 7, -1, -1, -1, -1},
{2, 6, 9, 2, 9, 1, 6, 7, 9, 0, 9, 3, 7, 3, 9, -1},
{7, 8, 0, 7, 0, 6, 6, 0, 2, -1, -1, -1, -1, -1, -1, -1},
{7, 3, 2, 6, 7, 2, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
{2, 3, 11, 10, 6, 8, 10, 8, 9, 8, 6, 7, -1, -1, -1, -1},
{2, 0, 7, 2, 7, 11, 0, 9, 7, 6, 7, 10, 9, 10, 7, -1},
{1, 8, 0, 1, 7, 8, 1, 10, 7, 6, 7, 10, 2, 3, 11, -1},
{11, 2, 1, 11, 1, 7, 10, 6, 1, 6, 7, 1, -1, -1, -1, -1},
{8, 9, 6, 8, 6, 7, 9, 1, 6, 11, 6, 3, 1, 3, 6, -1},
{0, 9, 1, 11, 6, 7, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
{7, 8, 0, 7, 0, 6, 3, 11, 0, 11, 6, 0, -1, -1, -1, -1},
{7, 11, 6, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
{7, 6, 11, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
{3, 0, 8, 11, 7, 6, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
{0, 1, 9, 11, 7, 6, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
{8, 1, 9, 8, 3, 1, 11, 7, 6, -1, -1, -1, -1, -1, -1, -1},
{10, 1, 2, 6, 11, 7, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
{1, 2, 10, 3, 0, 8, 6, 11, 7, -1, -1, -1, -1, -1, -1, -1},
{2, 9, 0, 2, 10, 9, 6, 11, 7, -1, -1, -1, -1, -1, -1, -1},
{6, 11, 7, 2, 10, 3, 10, 8, 3, 10, 9, 8, -1, -1, -1, -1},
{7, 2, 3, 6, 2, 7, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
{7, 0, 8, 7, 6, 0, 6, 2, 0, -1, -1, -1, -1, -1, -1, -1},
{2, 7, 6, 2, 3, 7, 0, 1, 9, -1, -1, -1, -1, -1, -1, -1},
{1, 6, 2, 1, 8, 6, 1, 9, 8, 8, 7, 6, -1, -1, -1, -1},
{10, 7, 6, 10, 1, 7, 1, 3, 7, -1, -1, -1, -1, -1, -1, -1},
{10, 7, 6, 1, 7, 10, 1, 8, 7, 1, 0, 8, -1, -1, -1, -1},
{0, 3, 7, 0, 7, 10, 0, 10, 9, 6, 10, 7, -1, -1, -1, -1},
{7, 6, 10, 7, 10, 8, 8, 10, 9, -1, -1, -1, -1, -1, -1, -1},
{6, 8, 4, 11, 8, 6, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
{3, 6, 11, 3, 0, 6, 0, 4, 6, -1, -1, -1, -1, -1, -1, -1},
{8, 6, 11, 8, 4, 6, 9, 0, 1, -1, -1, -1, -1, -1, -1, -1},
{9, 4, 6, 9, 6, 3, 9, 3, 1, 11, 3, 6, -1, -1, -1, -1},
{6, 8, 4, 6, 11, 8, 2, 10, 1, -1, -1, -1, -1, -1, -1, -1},
{1, 2, 10, 3, 0, 11, 0, 6, 11, 0, 4, 6, -1, -1, -1, -1},
{4, 11, 8, 4, 6, 11, 0, 2, 9, 2, 10, 9, -1, -1, -1, -1},
{10, 9, 3, 10, 3, 2, 9, 4, 3, 11, 3, 6, 4, 6, 3, -1},
{8, 2, 3, 8, 4, 2, 4, 6, 2, -1, -1, -1, -1, -1, -1, -1},
{0, 4, 2, 4, 6, 2, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
{1, 9, 0, 2, 3, 4, 2, 4, 6, 4, 3, 8, -1, -1, -1, -1},
{1, 9, 4, 1, 4, 2, 2, 4, 6, -1, -1, -1, -1, -1, -1, -1},
{8, 1, 3, 8, 6, 1, 8, 4, 6, 6, 10, 1, -1, -1, -1, -1},
{10, 1, 0, 10, 0, 6, 6, 0, 4, -1, -1, -1, -1, -1, -1, -1},
{4, 6, 3, 4, 3, 8, 6, 10, 3, 0, 3, 9, 10, 9, 3, -1},
{10, 9, 4, 6, 10, 4, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
{4, 9, 5, 7, 6, 11, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
{0, 8, 3, 4, 9, 5, 11, 7, 6, -1, -1, -1, -1, -1, -1, -1},
{5, 0, 1, 5, 4, 0, 7, 6, 11, -1, -1, -1, -1, -1, -1, -1},
{11, 7, 6, 8, 3, 4, 3, 5, 4, 3, 1, 5, -1, -1, -1, -1},
{9, 5, 4, 10, 1, 2, 7, 6, 11, -1, -1, -1, -1, -1, -1, -1},
{6, 11, 7, 1, 2, 10, 0, 8, 3, 4, 9, 5, -1, -1, -1, -1},
{7, 6, 11, 5, 4, 10, 4, 2, 10, 4, 0, 2, -1, -1, -1, -1},
{3, 4, 8, 3, 5, 4, 3, 2, 5, 10, 5, 2, 11, 7, 6, -1},
{7, 2, 3, 7, 6, 2, 5, 4, 9, -1, -1, -1, -1, -1, -1, -1},
{9, 5, 4, 0, 8, 6, 0, 6, 2, 6, 8, 7, -1, -1, -1, -1},
{3, 6, 2, 3, 7, 6, 1, 5, 0, 5, 4, 0, -1, -1, -1, -1},
{6, 2, 8, 6, 8, 7, 2, 1, 8, 4, 8, 5, 1, 5, 8, -1},
{9, 5, 4, 10, 1, 6, 1, 7, 6, 1, 3, 7, -1, -1, -1, -1},
{1, 6, 10, 1, 7, 6, 1, 0, 7, 8, 7, 0, 9, 5, 4, -1},
{4, 0, 10, 4, 10, 5, 0, 3, 10, 6, 10, 7, 3, 7, 10, -1},
{7, 6, 10, 7, 10, 8, 5, 4, 10, 4, 8, 10, -1, -1, -1, -1},
{6, 9, 5, 6, 11, 9, 11, 8, 9, -1, -1, -1, -1, -1, -1, -1},
{3, 6, 11, 0, 6, 3, 0, 5, 6, 0, 9, 5, -1, -1, -1, -1},
{0, 11, 8, 0, 5, 11, 0, 1, 5, 5, 6, 11, -1, -1, -1, -1},
{6, 11, 3, 6, 3, 5, 5, 3, 1, -1, -1, -1, -1, -1, -1, -1},
{1, 2, 10, 9, 5, 11, 9, 11, 8, 11, 5, 6, -1, -1, -1, -1},
{0, 11, 3, 0, 6, 11, 0, 9, 6, 5, 6, 9, 1, 2, 10, -1},
{11, 8, 5, 11, 5, 6, 8, 0, 5, 10, 5, 2, 0, 2, 5, -1},
{6, 11, 3, 6, 3, 5, 2, 10, 3, 10, 5, 3, -1, -1, -1, -1},
{5, 8, 9, 5, 2, 8, 5, 6, 2, 3, 8, 2, -1, -1, -1, -1},
{9, 5, 6, 9, 6, 0, 0, 6, 2, -1, -1, -1, -1, -1, -1, -1},
{1, 5, 8, 1, 8, 0, 5, 6, 8, 3, 8, 2, 6, 2, 8, -1},
{1, 5, 6, 2, 1, 6, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
{1, 3, 6, 1, 6, 10, 3, 8, 6, 5, 6, 9, 8, 9, 6, -1},
{10, 1, 0, 10, 0, 6, 9, 5, 0, 5, 6, 0, -1, -1, -1, -1},
{0, 3, 8, 5, 6, 10, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
{10, 5, 6, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
{11, 5, 10, 7, 5, 11, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
{11, 5, 10, 11, 7, 5, 8, 3, 0, -1, -1, -1, -1, -1, -1, -1},
{5, 11, 7, 5, 10, 11, 1, 9, 0, -1, -1, -1, -1, -1, -1, -1},
{10, 7, 5, 10, 11, 7, 9, 8, 1, 8, 3, 1, -1, -1, -1, -1},
{11, 1, 2, 11, 7, 1, 7, 5, 1, -1, -1, -1, -1, -1, -1, -1},
{0, 8, 3, 1, 2, 7, 1, 7, 5, 7, 2, 11, -1, -1, -1, -1},
{9, 7, 5, 9, 2, 7, 9, 0, 2, 2, 11, 7, -1, -1, -1, -1},
{7, 5, 2, 7, 2, 11, 5, 9, 2, 3, 2, 8, 9, 8, 2, -1},
{2, 5, 10, 2, 3, 5, 3, 7, 5, -1, -1, -1, -1, -1, -1, -1},
{8, 2, 0, 8, 5, 2, 8, 7, 5, 10, 2, 5, -1, -1, -1, -1},
{9, 0, 1, 5, 10, 3, 5, 3, 7, 3, 10, 2, -1, -1, -1, -1},
{9, 8, 2, 9, 2, 1, 8, 7, 2, 10, 2, 5, 7, 5, 2, -1},
{1, 3, 5, 3, 7, 5, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
{0, 8, 7, 0, 7, 1, 1, 7, 5, -1, -1, -1, -1, -1, -1, -1},
{9, 0, 3, 9, 3, 5, 5, 3, 7, -1, -1, -1, -1, -1, -1, -1},
{9, 8, 7, 5, 9, 7, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
{5, 8, 4, 5, 10, 8, 10, 11, 8, -1, -1, -1, -1, -1, -1, -1},
{5, 0, 4, 5, 11, 0, 5, 10, 11, 11, 3, 0, -1, -1, -1, -1},
{0, 1, 9, 8, 4, 10, 8, 10, 11, 10, 4, 5, -1, -1, -1, -1},
{10, 11, 4, 10, 4, 5, 11, 3, 4, 9, 4, 1, 3, 1, 4, -1},
{2, 5, 1, 2, 8, 5, 2, 11, 8, 4, 5, 8, -1, -1, -1, -1},
{0, 4, 11, 0, 11, 3, 4, 5, 11, 2, 11, 1, 5, 1, 11, -1},
{0, 2, 5, 0, 5, 9, 2, 11, 5, 4, 5, 8, 11, 8, 5, -1},
{9, 4, 5, 2, 11, 3, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
{2, 5, 10, 3, 5, 2, 3, 4, 5, 3, 8, 4, -1, -1, -1, -1},
{5, 10, 2, 5, 2, 4, 4, 2, 0, -1, -1, -1, -1, -1, -1, -1},
{3, 10, 2, 3, 5, 10, 3, 8, 5, 4, 5, 8, 0, 1, 9, -1},
{5, 10, 2, 5, 2, 4, 1, 9, 2, 9, 4, 2, -1, -1, -1, -1},
{8, 4, 5, 8, 5, 3, 3, 5, 1, -1, -1, -1, -1, -1, -1, -1},
{0, 4, 5, 1, 0, 5, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
{8, 4, 5, 8, 5, 3, 9, 0, 5, 0, 3, 5, -1, -1, -1, -1},
{9, 4, 5, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
{4, 11, 7, 4, 9, 11, 9, 10, 11, -1, -1, -1, -1, -1, -1, -1},
{0, 8, 3, 4, 9, 7, 9, 11, 7, 9, 10, 11, -1, -1, -1, -1},
{1, 10, 11, 1, 11, 4, 1, 4, 0, 7, 4, 11, -1, -1, -1, -1},
{3, 1, 4, 3, 4, 8, 1, 10, 4, 7, 4, 11, 10, 11, 4, -1},
{4, 11, 7, 9, 11, 4, 9, 2, 11, 9, 1, 2, -1, -1, -1, -1},
{9, 7, 4, 9, 11, 7, 9, 1, 11, 2, 11, 1, 0, 8, 3, -1},
{11, 7, 4, 11, 4, 2, 2, 4, 0, -1, -1, -1, -1, -1, -1, -1},
{11, 7, 4, 11, 4, 2, 8, 3, 4, 3, 2, 4, -1, -1, -1, -1},
{2, 9, 10, 2, 7, 9, 2, 3, 7, 7, 4, 9, -1, -1, -1, -1},
{9, 10, 7, 9, 7, 4, 10, 2, 7, 8, 7, 0, 2, 0, 7, -1},
{3, 7, 10, 3, 10, 2, 7, 4, 10, 1, 10, 0, 4, 0, 10, -1},
{1, 10, 2, 8, 7, 4, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
{4, 9, 1, 4, 1, 7, 7, 1, 3, -1, -1, -1, -1, -1, -1, -1},
{4, 9, 1, 4, 1, 7, 0, 8, 1, 8, 7, 1, -1, -1, -1, -1},
{4, 0, 3, 7, 4, 3, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
{4, 8, 7, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
{9, 10, 8, 10, 11, 8, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
{3, 0, 9, 3, 9, 11, 11, 9, 10, -1, -1, -1, -1, -1, -1, -1},
{0, 1, 10, 0, 10, 8, 8, 10, 11, -1, -1, -1, -1, -1, -1, -1},
{3, 1, 10, 11, 3, 10, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
{1, 2, 11, 1, 11, 9, 9, 11, 8, -1, -1, -1, -1, -1, -1, -1},
{3, 0, 9, 3, 9, 11, 1, 2, 9, 2, 11, 9, -1, -1, -1, -1},
{0, 2, 11, 8, 0, 11, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
{3, 2, 11, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
{2, 3, 8, 2, 8, 10, 10, 8, 9, -1, -1, -1, -1, -1, -1, -1},
{9, 10, 2, 0, 9, 2, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
{2, 3, 8, 2, 8, 10, 0, 1, 8, 1, 10, 8, -1, -1, -1, -1},
{1, 10, 2, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
{1, 3, 8, 9, 1, 8, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
{0, 9, 1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
{0, 3, 8, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
{-1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1}};


    #endregion

    Point3D cubeOrigin;
    double lowerBoundX, upperBoundX, lowerBoundY, upperBoundY, lowerBoundZ, upperBoundZ;

    #region Rotations
    double xRotation = 0.0;
    double yRotation = 0.0;
    double zRotation = 0.0;

    public double RotateX
    {
        get { return xRotation; }
        set { xRotation = value; }
    }

    public double RotateY
    {
        get { return yRotation; }
        set { yRotation = value; }
    }

    public double RotateZ
    {
        get { return zRotation; }
        set { zRotation = value; }
    }
    #endregion

    private Point3D[] Vertexes;
    private int[] Indexs;
    public void CreateSurface(double[, ,] Values, double threshold, int nRows, int nCols, int nStairs)
    {
        List<TRIANGLE> Triangles = new List<TRIANGLE>();
        double dX = (double)Values.GetLength(0) / (double)nRows;
        double dY = (double)Values.GetLength(1) / (double)nCols;
        double dZ = (double)Values.GetLength(2) / (double)nStairs;

        for (int i = 0; i < nRows - 1; i++)
        {
            for (int j = 0; j < nCols - 1; j++)
            {
                for (int m = 0; m < nStairs - 1; m++)
                {
                    GRIDCELL gc = new GRIDCELL(8);

                    gc.p[0].X = (i) * dX;
                    gc.p[0].Y = (j) * dY;
                    gc.p[0].Z = (m) * dZ;
                    gc.val[0] = Values[(int)gc.p[0].X, (int)gc.p[0].Y, (int)gc.p[0].Z];

                    gc.p[1].X = (i + 1) * dX;
                    gc.p[1].Y = (j) * dY;
                    gc.p[1].Z = (m) * dZ;
                    gc.val[1] = Values[(int)gc.p[1].X, (int)gc.p[1].Y, (int)gc.p[1].Z];

                    gc.p[2].X = (i + 1) * dX;
                    gc.p[2].Y = (j + 1) * dY;
                    gc.p[2].Z = (m) * dZ;
                    gc.val[2] = Values[(int)gc.p[2].X, (int)gc.p[2].Y, (int)gc.p[2].Z];

                    gc.p[3].X = (i) * dX;
                    gc.p[3].Y = (j + 1) * dY;
                    gc.p[3].Z = (m) * dZ;
                    gc.val[3] = Values[(int)gc.p[3].X, (int)gc.p[3].Y, (int)gc.p[3].Z];

                    gc.p[4].X = (i) * dX;
                    gc.p[4].Y = (j) * dY;
                    gc.p[4].Z = (m + 1) * dZ;
                    gc.val[4] = Values[(int)gc.p[4].X, (int)gc.p[4].Y, (int)gc.p[4].Z];

                    gc.p[5].X = (i + 1) * dX;
                    gc.p[5].Y = (j) * dY;
                    gc.p[5].Z = (m + 1) * dZ;
                    gc.val[5] = Values[(int)gc.p[5].X, (int)gc.p[5].Y, (int)gc.p[5].Z];

                    gc.p[6].X = (i + 1) * dX;
                    gc.p[6].Y = (j + 1) * dY;
                    gc.p[6].Z = (m + 1) * dZ;
                    gc.val[6] = Values[(int)gc.p[6].X, (int)gc.p[6].Y, (int)gc.p[6].Z];

                    gc.p[7].X = (i) * dX;
                    gc.p[7].Y = (j + 1) * dY;
                    gc.p[7].Z = (m + 1) * dZ;
                    gc.val[7] = Values[(int)gc.p[7].X, (int)gc.p[7].Y, (int)gc.p[7].Z];

                    TRIANGLE[] tOut = Polygonise(gc, threshold);

                    if (tOut != null)
                        Triangles.AddRange(tOut);

                }
            }
        }
        Point3D[] points = new Point3D[Triangles.Count * 3];
        int cc1 = 0;
        Indexs = new int[Triangles.Count * 4];
        int cc2 = 0;
        for (int i = 0; i < Triangles.Count; i++)
        {
            points[cc1] = (new Point3D(Triangles[i].p[0].X, Triangles[i].p[0].Y, Triangles[i].p[0].Z));
            points[cc1 + 1] = (new Point3D(Triangles[i].p[1].X, Triangles[i].p[1].Y, Triangles[i].p[1].Z));
            points[cc1 + 2] = (new Point3D(Triangles[i].p[2].X, Triangles[i].p[2].Y, Triangles[i].p[2].Z));

            Indexs[cc2] = cc1;
            Indexs[cc2 + 1] = cc1 + 1;
            Indexs[cc2 + 2] = cc1 + 2;
            Indexs[cc2 + 3] = cc1;

            cc1 += 3;
            cc2 += 4;
        }

        RemoveDuplicates(ref points, ref Indexs );

        double xx = 0;
        double yy = 0;
        double zz = 0;
        lowerBoundX = double.MaxValue; upperBoundX = double.MinValue; lowerBoundY = double.MaxValue; upperBoundY = double.MinValue; lowerBoundZ = double.MaxValue; upperBoundZ = double.MinValue;
        for (int i = 0; i < points.Length; i++)
        {
            xx += points[i].X;
            yy += points[i].Y;
            zz += points[i].Z;

            if (points[i].X > upperBoundX) upperBoundX = points[i].X;
            if (points[i].X < lowerBoundX) lowerBoundX = points[i].X;

            if (points[i].Y > upperBoundY) upperBoundY = points[i].Y;
            if (points[i].Y < lowerBoundY) lowerBoundY = points[i].Y;

            if (points[i].Z > upperBoundZ) upperBoundZ = points[i].Z;
            if (points[i].Z < lowerBoundZ) lowerBoundZ = points[i].Z;

        }
        lowerBoundX = 0;
        upperBoundX = Values.GetLength(0);

        lowerBoundY = 0;
        upperBoundY = Values.GetLength(1);

        lowerBoundZ = 0;
        upperBoundZ = Values.GetLength(2);

        cubeOrigin = new Point3D(xx / (double)points.Length, yy / (double)points.Length, zz / (double)points.Length);

      

        Vertexes = points.ToArray();
    }

    #region MarchingCubes
    private TRIANGLE[] Polygonise(GRIDCELL grid, double isolevel)
    {
        int i;
        int cubeindex;
        Point3D[] vertlist = new Point3D[12];

        /*
          Determine the index into the edge table which
          tells us which vertices are inside of the surface
       */
        cubeindex = 0;
        if (grid.val[0] < isolevel) cubeindex |= 1;
        if (grid.val[1] < isolevel) cubeindex |= 2;
        if (grid.val[2] < isolevel) cubeindex |= 4;
        if (grid.val[3] < isolevel) cubeindex |= 8;
        if (grid.val[4] < isolevel) cubeindex |= 16;
        if (grid.val[5] < isolevel) cubeindex |= 32;
        if (grid.val[6] < isolevel) cubeindex |= 64;
        if (grid.val[7] < isolevel) cubeindex |= 128;

        /* Cube is entirely in/out of the surface */
        if (edgeTable[cubeindex] == 0)
            return (null);

        /* Find the vertices where the surface intersects the cube */
        if ((edgeTable[cubeindex] & 1) != 0)
            vertlist[0] = VertexInterp(isolevel, grid.p[0], grid.p[1], grid.val[0], grid.val[1]);
        if ((edgeTable[cubeindex] & 2) != 0)
            vertlist[1] =
               VertexInterp(isolevel, grid.p[1], grid.p[2], grid.val[1], grid.val[2]);
        if (0 != (edgeTable[cubeindex] & 4))
            vertlist[2] =
               VertexInterp(isolevel, grid.p[2], grid.p[3], grid.val[2], grid.val[3]);
        if (0 != (edgeTable[cubeindex] & 8))
            vertlist[3] =
               VertexInterp(isolevel, grid.p[3], grid.p[0], grid.val[3], grid.val[0]);
        if (0 != (edgeTable[cubeindex] & 16))
            vertlist[4] =
               VertexInterp(isolevel, grid.p[4], grid.p[5], grid.val[4], grid.val[5]);
        if (0 != (edgeTable[cubeindex] & 32))
            vertlist[5] =
               VertexInterp(isolevel, grid.p[5], grid.p[6], grid.val[5], grid.val[6]);
        if (0 != (edgeTable[cubeindex] & 64))
            vertlist[6] =
               VertexInterp(isolevel, grid.p[6], grid.p[7], grid.val[6], grid.val[7]);
        if (0 != (edgeTable[cubeindex] & 128))
            vertlist[7] =
               VertexInterp(isolevel, grid.p[7], grid.p[4], grid.val[7], grid.val[4]);
        if (0 != (edgeTable[cubeindex] & 256))
            vertlist[8] =
               VertexInterp(isolevel, grid.p[0], grid.p[4], grid.val[0], grid.val[4]);
        if (0 != (edgeTable[cubeindex] & 512))
            vertlist[9] =
               VertexInterp(isolevel, grid.p[1], grid.p[5], grid.val[1], grid.val[5]);
        if (0 != (edgeTable[cubeindex] & 1024))
            vertlist[10] =
               VertexInterp(isolevel, grid.p[2], grid.p[6], grid.val[2], grid.val[6]);
        if (0 != (edgeTable[cubeindex] & 2048))
            vertlist[11] =
               VertexInterp(isolevel, grid.p[3], grid.p[7], grid.val[3], grid.val[7]);

        /* Create the triangle */
        List<TRIANGLE> triangles = new List<TRIANGLE>();
        for (i = 0; triTable[cubeindex, i] != -1; i += 3)
        {
            TRIANGLE triangle = new TRIANGLE(1);
            triangle.p[0] = vertlist[triTable[cubeindex, i]];
            triangle.p[1] = vertlist[triTable[cubeindex, i + 1]];
            triangle.p[2] = vertlist[triTable[cubeindex, i + 2]];
            triangles.Add(triangle);
        }

        return triangles.ToArray();
    }

    private void RemoveDuplicates(ref Point3D[] Vertexlist, ref int[] IndexList)
    {
        Point3D p2;

        int[] Indexs = new int[Vertexlist.Length];
        bool[] BadPoint = new bool[Vertexlist.Length];

        int VertexCount = Vertexlist.Length;
        int cc2 = 0;
        for (int i = 0; i < Vertexlist.Length; i++)
        {
            if (BadPoint[i] == false)
            {
                Point3D p = Vertexlist[i];
                Indexs[i] = cc2;
                for (int j = i + 1; j < Vertexlist.Length; j++)
                {
                    p2 = Vertexlist[j];
                    if (p.X == p2.X)
                        if (p.Y == p2.Y)
                            if (p.Z == p2.Z)
                            {
                                BadPoint[j] = true;
                                Indexs[j] = cc2;
                                VertexCount--;
                            }
                }
                cc2++;
            }
        }
        int cc = 0;
        Point3D[] NewPoints = new Point3D[VertexCount];
        for (int i = 0; i < Vertexlist.Length; i++)
        {
            if (BadPoint[i] == false)
            {
                NewPoints[cc] = Vertexlist[i];
                cc++;
            }
        }
        for (int i = 0; i < IndexList.Length; i++)
        {
            IndexList[i] = Indexs[IndexList[i]];
        }
        Vertexlist = NewPoints;

    }

    /*
       Linearly interpolate the position where an isosurface cuts
       an edge between two vertices, each with their own scalar value
    */
    Point3D VertexInterp(double isolevel, Point3D p1, Point3D p2, double valp1, double valp2)
    {
        double mu;
        Point3D p = new Point3D();

        if (Math.Abs(isolevel - valp1) < 0.00001)
            return (p1);
        if (Math.Abs(isolevel - valp2) < 0.00001)
            return (p2);
        if (Math.Abs(valp1 - valp2) < 0.00001)
            return (p1);
        mu = (isolevel - valp1) / (valp2 - valp1);
        p.X = p1.X + mu * (p2.X - p1.X);
        p.Y = p1.Y + mu * (p2.Y - p1.Y);
        p.Z = p1.Z + mu * (p2.Z - p1.Z);

        return (p);
    }
    #endregion

    #region PresentSpots
    public Point3D[] VertexList
    {
        get { return Vertexes; }
    }

    public int[] TriangleIndexs
    {
        get { return Indexs; }
    }

    public void CenterPoints()
    {
        for (int i = 0; i < Vertexes.Length; i++)
        {
            Vertexes[i].X = (Vertexes[i].X - cubeOrigin.X);
            Vertexes[i].Y = (Vertexes[i].Y - cubeOrigin.Y);
            Vertexes[i].Z = (Vertexes[i].Z - cubeOrigin.Z);
        }
        lowerBoundX = (lowerBoundX - cubeOrigin.X);
        upperBoundX = (upperBoundX - cubeOrigin.X);

        lowerBoundY = (lowerBoundY - cubeOrigin.Y);
        upperBoundY = (upperBoundY - cubeOrigin.Y);

        lowerBoundZ = (lowerBoundZ - cubeOrigin.Z);
        upperBoundZ = (upperBoundZ - cubeOrigin.Z);

        cubeOrigin.X = 0;
        cubeOrigin.Y = 0;
        cubeOrigin.Z = 0;

    }
    public void NormalizePoints()
    {
        NormalizePoints(1);
    }

    /// <summary>
    /// Normalizes the points to the new bound 
    /// </summary>
    /// <param name="TargetBound"></param>
    public void NormalizePoints(double TargetBound)
    {
        double MaxBound = 0;
        if (Math.Abs(lowerBoundX - cubeOrigin.X) > MaxBound) MaxBound = Math.Abs(lowerBoundX - cubeOrigin.X);
        if (Math.Abs(upperBoundX - cubeOrigin.X) > MaxBound) MaxBound = Math.Abs(upperBoundX - cubeOrigin.X);

        if (Math.Abs(lowerBoundY - cubeOrigin.Y) > MaxBound) MaxBound = Math.Abs(lowerBoundY - cubeOrigin.Y);
        if (Math.Abs(upperBoundY - cubeOrigin.Y) > MaxBound) MaxBound = Math.Abs(upperBoundY - cubeOrigin.Y);

        if (Math.Abs(lowerBoundZ - cubeOrigin.Z) > MaxBound) MaxBound = Math.Abs(lowerBoundZ - cubeOrigin.Z);
        if (Math.Abs(upperBoundZ - cubeOrigin.Z) > MaxBound) MaxBound = Math.Abs(upperBoundZ - cubeOrigin.Z);

        for (int i = 0; i < Vertexes.Length; i++)
        {
            Vertexes[i].X = (Vertexes[i].X - cubeOrigin.X) / MaxBound * TargetBound + cubeOrigin.X ;
            Vertexes[i].Y = (Vertexes[i].Y - cubeOrigin.Y) / MaxBound * TargetBound + cubeOrigin.Y;
            Vertexes[i].Z = (Vertexes[i].Z - cubeOrigin.Z) / MaxBound * TargetBound + cubeOrigin.Z;
        }
        lowerBoundX = (lowerBoundX - cubeOrigin.X) / MaxBound * TargetBound + cubeOrigin.X;
        upperBoundX = (upperBoundX - cubeOrigin.X) / MaxBound * TargetBound + cubeOrigin.X;

        lowerBoundY = (lowerBoundY - cubeOrigin.Y) / MaxBound * TargetBound + cubeOrigin.Y;
        upperBoundY = (upperBoundY - cubeOrigin.Y) / MaxBound * TargetBound + cubeOrigin.Y;

        lowerBoundZ = (lowerBoundZ - cubeOrigin.Z) / MaxBound * TargetBound + cubeOrigin.Z;
        upperBoundZ = (upperBoundZ - cubeOrigin.Z) / MaxBound * TargetBound + cubeOrigin.Z;
       
    }
    public void CenterAndNormalizePoints()
    {
        CenterAndNormalizePoints(1);
    }

    /// <summary>
    /// Centers the points and then scales them to the new Bound Value
    /// </summary>
    /// <param name="TargetBound"></param>
    public void CenterAndNormalizePoints(double TargetBound)
    {
        double MaxBound = 0;
        if (Math.Abs(lowerBoundX - cubeOrigin.X) > MaxBound) MaxBound = Math.Abs(lowerBoundX - cubeOrigin.X);
        if (Math.Abs(upperBoundX  - cubeOrigin.X) > MaxBound) MaxBound = Math.Abs(upperBoundX  - cubeOrigin.X);

        if (Math.Abs(lowerBoundY - cubeOrigin.Y) > MaxBound) MaxBound = Math.Abs(lowerBoundY - cubeOrigin.Y);
        if (Math.Abs(upperBoundY - cubeOrigin.Y) > MaxBound) MaxBound = Math.Abs(upperBoundY - cubeOrigin.Y);

        if (Math.Abs(lowerBoundZ - cubeOrigin.Z) > MaxBound) MaxBound = Math.Abs(lowerBoundZ - cubeOrigin.Z);
        if (Math.Abs(upperBoundZ - cubeOrigin.Z) > MaxBound) MaxBound = Math.Abs(upperBoundZ - cubeOrigin.Z);

        for (int i = 0; i < Vertexes.Length; i++)
        {
            Vertexes[i].X = (Vertexes[i].X - cubeOrigin.X) / MaxBound*TargetBound ;
            Vertexes[i].Y = (Vertexes[i].Y - cubeOrigin.Y) / MaxBound*TargetBound ;
            Vertexes[i].Z = (Vertexes[i].Z - cubeOrigin.Z) / MaxBound*TargetBound ;
        }
        lowerBoundX =(lowerBoundX - cubeOrigin.X )/MaxBound *TargetBound ;
        upperBoundX =(upperBoundX - cubeOrigin.X )/MaxBound *TargetBound ;

        lowerBoundY = (lowerBoundY - cubeOrigin.Y) / MaxBound * TargetBound;
        upperBoundY = (upperBoundY - cubeOrigin.Y) / MaxBound * TargetBound;

        lowerBoundZ = (lowerBoundZ - cubeOrigin.Z) / MaxBound * TargetBound;
        upperBoundZ = (upperBoundZ - cubeOrigin.Z) / MaxBound * TargetBound;

        cubeOrigin.X = 0;
        cubeOrigin.Y = 0;
        cubeOrigin.Z = 0;
    }

    public Bitmap drawPoints(int Width, int Height)
    {
        //Vars
        Point[] point3D = new Point[Vertexes.Length]; //Will be actual 2D drawing points

        //Set up the cube
        Point3D[] cubePoints = new Point3D[Vertexes.Length];
        for (int i = 0; i < cubePoints.Length; i++)
        {
            cubePoints[i] = new Point3D(Vertexes[i].X, Vertexes[i].Y, Vertexes[i].Z);
        }

        Point3D point0 = new Point3D(0, 0, 0);
        //Apply Rotations, moving the cube to a corner then back to middle
        cubePoints = Point3D.Translate(cubePoints, cubeOrigin, point0);
        cubePoints = Point3D.RotateXAxis(cubePoints, xRotation); //The order of these
        cubePoints = Point3D.RotateYAxis(cubePoints, yRotation); //rotations is the source
        cubePoints = Point3D.RotateZAxis(cubePoints, zRotation); //of Gimbal Lock
        cubePoints = Point3D.Translate(cubePoints, point0, cubeOrigin);

        //Convert 3D Points to 2D
        Point3D vec;
        for (int i = 0; i < point3D.Length; i++)
        {
            vec = cubePoints[i];
            point3D[i].X = (int)((double)(vec.X - lowerBoundX) / (upperBoundX - lowerBoundX) * Width);
            point3D[i].Y = (int)((double)(vec.Y - lowerBoundY) / (upperBoundY - lowerBoundY) * Height);
        }

        //Now to plot out the points
        Rectangle bounds = new Rectangle(0, 0, Width, Height);

        Bitmap tmpBmp = new Bitmap(bounds.Width, bounds.Height);
        Graphics g = Graphics.FromImage(tmpBmp);
        g.Clear(Color.White);
        for (int i = 0; i < point3D.Length; i++)
        {
            try
            {
                tmpBmp.SetPixel((int)point3D[i].X, (int)point3D[i].Y, Color.Red);
            }
            catch { }
        }
        g.Dispose(); //Clean-up

        return tmpBmp;
    }

    public Bitmap drawLines(int Width, int Height)
    {
        //Vars
        Point[] point3D = new Point[Vertexes.Length]; //Will be actual 2D drawing points

        //Set up the cube
        Point3D[] cubePoints = new Point3D[Vertexes.Length];
        for (int i = 0; i < cubePoints.Length; i++)
        {
            cubePoints[i] = new Point3D(Vertexes[i].X, Vertexes[i].Y, Vertexes[i].Z);
        }

        Point3D point0 = new Point3D(0, 0, 0);
        //Apply Rotations, moving the cube to a corner then back to middle
        cubePoints = Point3D.Translate(cubePoints, cubeOrigin, point0);
        cubePoints = Point3D.RotateXAxis(cubePoints, xRotation); //The order of these
        cubePoints = Point3D.RotateYAxis(cubePoints, yRotation); //rotations is the source
        cubePoints = Point3D.RotateZAxis(cubePoints, zRotation); //of Gimbal Lock
        cubePoints = Point3D.Translate(cubePoints, point0, cubeOrigin);

        //Convert 3D Points to 2D
        Point3D vec;
        for (int i = 0; i < point3D.Length; i++)
        {
            vec = cubePoints[i];

            point3D[i].X = (int)((double)(vec.X - lowerBoundX) / (upperBoundX - lowerBoundX) * Width);
            point3D[i].Y = (int)((double)(vec.Y - lowerBoundY) / (upperBoundY - lowerBoundY) * Height);
        }

        //Now to plot out the points
        Rectangle bounds = new Rectangle(0, 0, Width, Height);

        Bitmap tmpBmp = new Bitmap(bounds.Width, bounds.Height);
        Graphics g = Graphics.FromImage(tmpBmp);
        g.Clear(Color.White);
        int a;
        int b;
        int c;
        int d;
        for (int i = 0; i < Indexs.Length; i += 4)
        {
            try
            {
                //if (Show[i] == true && Show[i + 1] == true && Show[i + 2] == true)
                {
                    a = Indexs[i];
                    b = Indexs[i + 1];
                    c = Indexs[i + 2];
                    d = Indexs[i + 3];
                    g.DrawLine(Pens.Red, point3D[a].X, point3D[a].Y, point3D[b].X, point3D[b].Y);
                    g.DrawLine(Pens.Red, point3D[b].X, point3D[b].Y, point3D[c].X, point3D[c].Y);
                    g.DrawLine(Pens.Red, point3D[c].X, point3D[c].Y, point3D[d].X, point3D[d].Y);
                }
            }
            catch { }
        }
        g.Dispose(); //Clean-up

        return tmpBmp;
    }

    //Finds the othermost points. Used so when the cube is drawn on a bitmap,
    //the bitmap will be the correct size
    public static Rectangle getBounds(PointF[] points)
    {
        double left = points[0].X;
        double right = points[0].X;
        double top = points[0].Y;
        double bottom = points[0].Y;
        for (int i = 1; i < points.Length; i++)
        {
            if (points[i].X < left)
                left = points[i].X;
            if (points[i].X > right)
                right = points[i].X;
            if (points[i].Y < top)
                top = points[i].Y;
            if (points[i].Y > bottom)
                bottom = points[i].Y;
        }

        return new Rectangle(0, 0, (int)Math.Round(right - left), (int)Math.Round(bottom - top));
    }
    #endregion
}