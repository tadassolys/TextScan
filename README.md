# TextScan - OCR Screenshot Tool

TextScan is a lightweight Windows desktop application that allows users to capture screenshots of any area on their screen and extract text using Optical Character Recognition (OCR) technology.

## Features

- **Screenshot Capture**: Select any area of your screen with a simple drag-and-drop interface
- **OCR Processing**: Extract text from images using Tesseract OCR engine
- **System Tray Integration**: Run in the background with minimal resource usage
- **User-Friendly Interface**: Simple, intuitive design for quick text extraction

## Usage

1. Launch the application
2. Click the "Capture Screenshot" button
3. Select the area containing text you want to extract
4. The extracted text will appear in the text box
5. Copy the text as needed

### System Tray Features

- The application minimizes to the system tray when you close it
- Double-click the tray icon to open the application
- Right-click the tray icon for additional options:
  - Open: Show the main application window
  - Capture: Start a new screenshot capture directly
  - Exit: Completely close the application

## Building from Source

1. Clone the repository
2. Open the solution in Visual Studio 2019 or newer
3. Restore NuGet packages (needs Tesseract package)
4. Create tessdata folder with preferred language e.g. "eng.traineddata" and store it in the output directory
5. Build the solution

## Dependencies

- [Tesseract OCR Engine](https://github.com/tesseract-ocr/tesseract)
- System.Drawing
- System.Windows.Forms


