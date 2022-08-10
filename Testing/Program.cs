using OMSharp;

Camera camera = new Camera();
await camera.SetCameraMode(Camera.CamMode.rec, Camera.RecResolution.r0640x0480);
await camera.StartLiveView(200);
Console.ReadKey();
await camera.StopLiveView();