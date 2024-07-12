# TomographicTests
A small program to test code to do tomographic projects and backprojections on 2D and 3D phantoms.  It is designed to be an educational tool only.


# Tomographic Projection and Backprojection Program

This repository contains a simple educational program written in C# that performs tomographic projection and backprojection. The program demonstrates the fundamental concepts of tomography, which is widely used in medical imaging and other fields.

## Features

###2D
- **Tomographic Projection**: Simulate the projection of a 2D object.
- **Backprojection**: Reconstruct the 2D object from its projections.
- **Visualization**: Display the original object, its projections, and the reconstructed image.

###3D
- **Tomographic Projection**: Simulate the projection of a 3D object.
- **Backprojection**: Reconstruct the 3D object from its projections.
- **Visualization**: Display the original object, its projections, and the reconstructed image.
- **Marching Cubes**: Display a nice reconstruction of the object using marching cubes to indicate the segmented boundries.


## Requirements

- .NET Framework
- 32-bit FFTW library

## Installation

1. **Clone the repository**:

   ```bash
   git clone https://github.com/drAshcroft/TomographicTests
   cd TomographicTests
   ```

2. **Download and Install FFTW Library**:

   - Download the 32-bit FFTW library from [here](http://www.fftw.org/install/windows.html).
   - Extract the downloaded files.
   - Copy the necessary DLL files (e.g., `libfftw3-3.dll`) to the `bin` folder of this project.

4. **Restore NuGet packages**:

   ```bash
   dotnet restore
   ```

## Usage

1. **Build the project**:

   ```bash
   dotnet build
   ```

2. **Run the program**:

   ```bash
   dotnet run
   ```

3. **Follow the on-screen instructions** to visualize the tomographic projections and backprojections.
 

## License

This project is licensed under the MIT License. See the [LICENSE](LICENSE) file for details.

## Acknowledgements

- [FFTW Library](http://www.fftw.org/) for providing the fast Fourier transform library.

## Contributing

Contributions are welcome! Please create an issue or submit a pull request.

 

---

By following these instructions, you should be able to run the program and explore the concepts of tomographic projection and backprojection. Happy learning!