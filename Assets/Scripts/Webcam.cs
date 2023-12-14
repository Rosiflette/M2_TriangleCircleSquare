using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Emgu.CV;
using Emgu.CV.Structure;
using UnityEngine.UI;
using System.Drawing;

public class Webcam : MonoBehaviour
{
    VideoCapture capture;
    Mat frame;
    Texture2D t;

    RawImage webcamScreen;

    void Start()
    {
        webcamScreen = gameObject.GetComponent<RawImage>();

        frame = new();
        capture = new();
        capture.ImageGrabbed += HandleWebcamQueryFrame;

        t = new Texture2D((int)webcamScreen.rectTransform.rect.width, (int)webcamScreen.rectTransform.rect.height, TextureFormat.RGBA32, false);
        gameObject.GetComponent<RawImage>().texture = t;

        capture.Start();
    }

    void OnDestroy()
    {
        capture.Stop();
        capture.Dispose();
    }

    void HandleWebcamQueryFrame(object sender, System.EventArgs e)
    {
        capture.Retrieve(frame);
    }

    void Update()
    {
        if (capture.IsOpened)
        {
            DisplayFrameOnPlane();
        }
    }

    void DisplayFrameOnPlane()
    {
        Mat currentFrame = frame.Clone();
        print(currentFrame.Size);
        CvInvoke.Resize(currentFrame, currentFrame, new Size(t.width, t.height));
        CvInvoke.CvtColor(currentFrame, currentFrame, Emgu.CV.CvEnum.ColorConversion.Bgr2Rgba);
        CvInvoke.Flip(currentFrame, currentFrame, Emgu.CV.CvEnum.FlipType.Horizontal);
        CvInvoke.Flip(currentFrame, currentFrame, Emgu.CV.CvEnum.FlipType.Vertical);

        t.LoadRawTextureData(currentFrame.DataPointer, currentFrame.Width * currentFrame.Height * 4);
        t.Apply();
    }

}
