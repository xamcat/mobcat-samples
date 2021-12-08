###### Sample: Machine learning in Xamarin with ONNX Runtime

## Overview
This first-principles example demonstrates use of [ONNX Runtime](https://onnxruntime.ai/) for on-device inferencing in a Xamarin.Forms app. An existing open-source image classification model ([MobileNet](https://github.com/onnx/models/blob/master/vision/classification/mobilenet)) from the [ONNX Model Zoo](https://github.com/onnx/models#onnx-model-zoo) has been used along with the [test image](https://github.com/microsoft/onnxruntime/raw/master/csharp/sample/Microsoft.ML.OnnxRuntime.ResNet50v2Sample/dog.jpeg) from an [existing sample](https://github.com/microsoft/onnxruntime/tree/master/csharp/sample/Microsoft.ML.OnnxRuntime.ResNet50v2Sample).

The app classifies the primary object in the [test image](https://github.com/microsoft/onnxruntime/raw/master/csharp/sample/Microsoft.ML.OnnxRuntime.ResNet50v2Sample/dog.jpeg), a golden retriever in this case, and displays the result.

![The inference result displayed in an alert](Illustrations/inference_result.png "inference result displayed in an alert")

The intent is to provide a helpful on-ramp for those looking to leverage [ONNX Runtime](https://onnxruntime.ai/) in their Xamarin.Forms apps. Be sure to checkout the official [getting started](https://onnxruntime.ai/docs/get-started/with-csharp.html) and [tutorial](https://onnxruntime.ai/docs/tutorials/api-basics.html) content as well as the [Xamarin specific samples](https://github.com/microsoft/onnxruntime-inference-examples/tree/main/mobile/examples/Xamarin).

## Getting Started

This sample should build and run from [Visual Studio](https://www.visualstudio.com) with no additional setup. You can clone (or download) the [mobcat-samples repo](https://github.com/xamcat/mobcat-samples) then open **InferenceSample.sln**

## Known Issues

### [MissingMethodException](https://docs.microsoft.com/dotnet/api/system.missingmethodexception) related to [ReadOnlySpan&lt;T>](https://docs.microsoft.com/dotnet/api/system.readonlyspan-1)

In [Visual Studio 2022](https://visualstudio.microsoft.com), [Hot Reload](https://docs.microsoft.com/xamarin/xamarin-forms/xaml/hot-reload) loads some additional dependencies including [System.Memory](https://www.nuget.org/packages/System.Memory) and [System.Buffers](https://www.nuget.org/packages/System.Buffers) which may cause conflicts with packages such as [ONNX Runtime](https://www.nuget.org/packages/Microsoft.ML.OnnxRuntime.Managed). The workaround is to [Disable Hot Reload](https://docs.microsoft.com/xamarin/xamarin-forms/xaml/hot-reload#enable-xaml-hot-reload-for-xamarinforms) until this [issue](https://developercommunity.visualstudio.com/t/bug-in-visual-studio-2022-xamarin-signalr-method-n/1528510#T-N1585809) has been addressed.