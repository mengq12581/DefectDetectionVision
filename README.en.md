# VisionDemo2026

[简体中文](README.md) | **English**

`VisionDemo2026` is a **ring-shaped part defect inspection application** built with **C# WinForms + HALCON**. It uses traditional machine vision techniques for image acquisition, preprocessing, region segmentation, geometric feature extraction, OK/NG classification, result annotation, and report export.

The application supports individual local images, image folders, USB cameras, and GigE industrial cameras. It provides a practical reference for HALCON integration, WinForms vision applications, and defect inspection workflows. The application interface, annotations, and exported text reports are currently in Chinese.

## Features

### Image input and acquisition

- Open an individual image and inspect it automatically; the Previous and Next buttons are disabled in single-image mode.
- Load a folder in filename order and navigate through images with wraparound.
- Save the most recent image or folder path and restore the corresponding source type at startup.
- Use the `MediaFoundation` interface to test a built-in laptop camera or a standard USB camera.
- Search for and open GigE Vision industrial cameras through `GigEVision2`.
- Search for cameras, select devices, set exposure, capture a single frame, and start or stop continuous acquisition.
- Inspect immediately after single-frame acquisition; preview continuously and inspect the last frame when acquisition stops.
- Save the camera interface, display name, original HALCON device identifier, and exposure value.

### Image processing and inspection

- Brightness and contrast adjustment.
- CLAHE local contrast enhancement.
- Gaussian noise reduction.
- Manual, Otsu automatic, and adaptive threshold segmentation.
- Morphological opening to remove small noise regions and closing to bridge small gaps.
- Canny edge extraction.
- Connected-component analysis and candidate filtering with a fixed area range.
- Interactive rectangular ROI drawing and inspection within the ROI.
- Extraction of area, perimeter, center coordinates, bounding rectangle, circularity, solidity, extent, hole count, and rotation-invariant aspect ratio.
- Angular analysis of inner and outer radii to detect small notches that do not cut through the ring wall.
- Combined OK/NG classification using area, solidity, circularity, hole count, aspect ratio, and local notches, with defect reasons recorded in reports.

### Display and export

- Preserve image proportions with HALCON `HWindowControl` to avoid stretching.
- Separate tabs for annotated results, the binary mask, and Canny edges.
- Green annotations for acceptable parts, red for defective parts, and yellow for the ROI.
- Chinese annotations showing area, solidity, and circularity.
- Export annotated images, mask images, and Chinese text inspection reports.

## Requirements

| Component | Requirement |
| --- | --- |
| Operating system | Windows 10 / Windows 11, 64-bit |
| Development environment | Visual Studio 2022 |
| Framework | .NET Framework 4.7.2 |
| Vision library | HALCON 24.05, 64-bit |
| UI library | SunnyUI 3.9.8 |
| Target platform | x64 |
| Local camera interface | HALCON MediaFoundation |
| Industrial camera interface | HALCON GigEVision2 |

HALCON, the application target platform, and the camera acquisition interface must have matching architectures. The project is configured for `x64`.

## Quick start

1. Open `VisionDemo2026.sln` in Visual Studio 2022.
2. Check that the `halcondotnet.dll` reference resolves to your local HALCON installation.
3. Select the `x64` target platform.
4. Build and run the application.
5. Choose Open Image (打开图片), Open Image Folder (打开图片文件夹), or use the camera controls in the upper-right corner.
6. Inspection runs automatically after loading an image or capturing a single frame. Adjust parameters or click Start Inspection (开始检测) to run it again.
7. To restrict inspection, click Draw ROI (绘制 ROI) and drag a rectangle in the image window. Click Reset ROI (重置 ROI) to inspect the whole image again.
8. Click Export Inspection Report (导出检测报告) to save the results.

## Camera setup

### Local cameras

1. Select Local Camera (本地相机) under Camera Interface (相机接口), corresponding to HALCON `MediaFoundation`.
2. Click Search Cameras (搜索相机).
3. Select a built-in camera or USB camera from the device list.
4. Use Single Acquisition (单次采集) to capture and inspect one frame, or Continuous Acquisition (连续采集) for live preview.
5. Click Stop Acquisition (停止采集) to stop the preview and inspect the last saved frame.

If no device is found, check Windows camera privacy permissions and make sure another application is not using the camera exclusively.

### GigE industrial cameras

1. Select Network Camera (网络相机), corresponding to HALCON `GigEVision2`.
2. Configure the camera network adapter, IP address, and HALCON GigEVision2 interface.
3. Click Search Cameras (搜索相机) and select a device.
4. Enter an exposure value supported by the camera, then acquire images.

Exposure parameter names, units, and valid ranges vary by interface and manufacturer. The camera helper tries MediaFoundation and common GenICam exposure parameters in sequence and displays a Chinese message if they are unsupported.

## Inspection pipeline

```text
Local image / camera image
  -> Brightness and contrast adjustment
  -> Grayscale conversion
  -> CLAHE local enhancement (optional)
  -> Gaussian filtering
  -> Manual / Otsu / adaptive threshold segmentation
  -> Morphological opening and closing
  -> ROI restriction (optional)
  -> Connected-component analysis
  -> Candidate area filtering (fixed range: 1000–8000)
  -> Feature extraction: area, solidity, circularity, hole count,
     rotation-invariant aspect ratio, and local inner/outer radii
  -> Combined acceptance checks
  -> Result annotation, statistics, and report export
```

The candidate area range of `1000–8000` is defined in the detection helper and only removes small noise regions and large background regions. The area settings in the UI define the acceptance range for parts; they do not change the candidate search range.

The `Canny` edge image is generated separately from the Gaussian-filtered grayscale image for visualization and parameter adjustment. It does not currently generate candidate regions or directly affect OK/NG classification.

## Default acceptance rules

| Feature | Default acceptance range | Description |
| --- | --- | --- |
| Area | 4000–5500 | NG if below the minimum or above the maximum |
| Solidity | 80%–90% | NG if outside this inclusive range |
| Circularity | At least the UI threshold | NG if below the minimum |
| Hole count | Exactly 1 | A through-crack or break can eliminate the enclosed hole |
| Aspect ratio | No greater than the UI limit | Uses the minimum-area rotated bounding rectangle to identify elliptical deformation |
| Local inner/outer notches | Fixed rule: radius deviation of at least 2 px across 3–30 consecutive angular bins | NG when the outer radius locally decreases or the inner radius locally increases |

Area, solidity, circularity, hole count, and aspect ratio must all pass for a part to be acceptable. Rings with an intact hole and otherwise acceptable geometry are also checked for local notches. Any failed check produces an NG result.

### Classification logic

Threshold segmentation, morphology, and connected-component analysis produce candidate parts. The fixed area range of `1000–8000` excludes obvious small noise regions and large background regions. Candidates outside the product acceptance range are retained, measured, and marked NG so that they remain visible and can be exported.

| Feature | Calculation | NG condition | Main purpose |
| --- | --- | --- | --- |
| Area | Number of pixels in the region | Outside the accepted area range | Missing material, excess material, or overall size anomalies |
| Solidity | HALCON `Convexity` | Outside the configured minimum/maximum range | Significant indentations or incomplete contours |
| Circularity | HALCON `Circularity` | Below the circularity threshold | Non-circular shapes or local contour anomalies |
| Hole count | HALCON `ConnectAndHoles` | Not equal to `1` | Through-cracks, breaks, missing holes, or extra holes |
| Aspect ratio | Major/minor axis ratio of the minimum-area rotated bounding rectangle | Above the maximum aspect ratio | Elliptical or stretched deformation, independent of part rotation |
| Outer notch | Divide the candidate region before closing into 180 angular bins, measure the outermost radius, and compare it with the median radius | Inward radius deviation of at least 2 px across 3–30 consecutive bins | Outer-edge chips and small notches that do not cut through the ring wall |
| Inner notch | Measure the innermost radius in the same angular bins and compare it with the median radius | Outward radius deviation of at least 2 px across 3–30 consecutive bins | Inner-edge notches and local missing material |

Local notch detection uses the region after opening but before closing, preventing closing from filling small notches. The region after closing is still used for stable part localization. Notch thresholds are fixed in code and are not exposed in the UI. Reports record the Chinese defect labels for outer or inner notches (外圈缺口 / 内圈缺口) and their depths.

### Supported ring defect cases

| Defect type | Current detection rule |
| --- | --- |
| Through-cracks and breaks | A crack connecting the outer boundary to the inner hole can change the enclosed hole count from `1` to `0` |
| Small outer-edge notches | A local reduction in outer radius relative to the median produces an outer-notch result |
| Small inner-edge notches | A local increase in inner radius relative to the median produces an inner-notch result |
| Elliptical or stretched deformation | Increased rotated aspect ratio, usually accompanied by reduced circularity |
| Significant missing material, chips, or contour indentations | Reduced area and potentially reduced solidity or circularity |
| Excess material, attached parts, or protrusions | Increased area or abnormal circularity, solidity, or aspect ratio |
| Missing or extra holes | Hole count differs from `1` |

### Current limitations

The project targets ring defects that noticeably change the contour or topology. The local notch rules cover inner- and outer-edge notches with depths of approximately 2 px or more. Smaller notches, surface scratches, stains, color differences, and small defects that do not alter the binary contour may be missed. Covering these cases would require additional methods within the ring ROI, such as local grayscale differences, edge continuity analysis, template comparison, or deep learning detection.

## Parameters

| Parameter | Purpose |
| --- | --- |
| Brightness | Adjust overall image brightness; 50 is the default baseline |
| Contrast | Adjust foreground/background grayscale contrast; 50 is the default baseline |
| Local contrast enhancement | Enhance local details using HALCON CLAHE |
| Gaussian filter level | Control smoothing strength; higher levels use larger kernels |
| Threshold mode | Select manual, Otsu automatic, or adaptive thresholding |
| Manual threshold | Used only in manual threshold mode |
| Adaptive block level | Control the background estimation window for dynamic thresholding |
| Opening radius | Remove isolated small regions and noise |
| Closing radius | Bridge small gaps and smooth target regions |
| Canny low threshold | Control edge detection sensitivity |
| Minimum/maximum accepted area | Define the accepted part area range |
| Minimum/maximum solidity | Define the accepted solidity range |
| Minimum circularity | Define the lowest accepted circularity |
| Maximum aspect ratio | Limit elliptical deformation; 135% means the major axis is at most 1.35 times the minor axis |

## Sample images

The project includes 8 static inspection samples in:

```text
VisionDemo2026\SampleImages
```

Each image contains 15–20 separate, non-overlapping ring parts and a small amount of noise. Samples cover four categories:

- Acceptable rings with similar area, solidity, and circularity.
- Cracked or broken rings whose overall bounding rectangles are similar to acceptable parts.
- Deformed rings with stretched or distorted contours.
- Rings with small inner- or outer-edge notches that retain their overall dimensions and enclosed hole count.

`sample_manifest.csv` records the class and center coordinates of each sample part. The application does not generate temporary simulated images when no image has been loaded.

To regenerate the same sample set, run this command from the solution directory:

```powershell
powershell.exe -NoProfile -ExecutionPolicy Bypass -File .\Tools\Generate-SampleImages.ps1
```

The script uses a fixed random seed by default. Pass `-Seed` to generate another set under the same constraints.

## Configuration and output

At runtime, the application creates these paths under the executable directory:

```text
Config\LastImageSource.json   # Most recent image or folder source
Config\CameraConfig.json     # Camera interface, device identifier, and exposure
reports\                     # Exported inspection results
```

The report directory contains:

- `*_result.png`: annotated result image with OK/NG labels, Chinese feature text, and ROI.
- `*_mask.png`: binary mask after threshold segmentation and morphology.
- `*_report.txt`: inspection summary, parameters, and per-part measurements with Chinese field names.

Single-image configuration restores the specified image and keeps navigation disabled. Folder configuration rescans the directory, sorts files by name, and starts from the first image. If a saved path is invalid, the application prompts the user to choose another source.

## Project structure

```text
VisionDemo2026/
├─ VisionDemo2026.sln
├─ README.md                         # Chinese documentation
├─ README.en.md                      # English documentation
├─ Tools/
│  └─ Generate-SampleImages.ps1
└─ VisionDemo2026/
   ├─ Program.cs
   ├─ Form1.cs                       # UI interaction, camera workflow, results, reports
   ├─ Form1.Designer.cs              # Designer-compatible controls and layout
   ├─ Helper/
   │  ├─ DefectDetectionHelper.cs    # Inspection pipeline and classification rules
   │  ├─ HalconCameraHelper.cs       # Camera discovery, opening, exposure, acquisition
   │  ├─ HalconImageHelper.cs        # Image loading, navigation, display, disposal
   │  ├─ HalconImageProcessHelper.cs # Image enhancement and filtering
   │  ├─ HalconRegionHelper.cs       # Thresholding, morphology, region features
   │  ├─ HalconVisionHelper.cs       # ROI, template, OCR, and metrology helpers
   │  ├─ HalconCalibrationHelper.cs  # Affine mapping from pixel to robot coordinates
   │  └─ SimpleJsonHelper.cs         # Image source and camera configuration persistence
   └─ SampleImages/                  # Static samples and class manifest
```

## Notes

- Default parameters are tuned for the included sample images. Retune segmentation and acceptance ranges when image resolution, lighting, background, or part size changes.
- The current algorithm uses traditional rules based on region geometry. It targets ring breaks, notches, and obvious deformation; it is not designed to directly handle complex textures or unknown defect classes.
- Continuous camera acquisition updates images through the UI thread. Acquisition handles, `HImage`, and `HObject` resources are released when acquisition stops or the form closes.
- The existing helpers provide a starting point for extensions such as shape matching, metrology models, calibration, deep learning classification, anomaly detection, or robot communication.
