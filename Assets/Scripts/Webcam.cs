using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Emgu.CV;
using Emgu.CV.Structure;
using UnityEngine.UI;
using System.Drawing;
using Emgu.CV.Util;

public class Webcam : MonoBehaviour
{
    [SerializeField] float contourThreshold = 5f;
    [SerializeField, Range(0, 25)] int erode, dilate;

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

    Mat ShapeDetection(Mat frame)
    {
        Image<Gray, byte> temp = frame.ToImage<Gray, byte>().ThresholdBinaryInv(new Gray(125), new Gray(255));
        temp._Erode(erode);
        temp._Dilate(dilate);
        //Mat currentFrame = temp.Convert<Bgr, byte>().Mat.Clone();
        Mat currentFrame = frame.Clone();

        VectorOfVectorOfPoint contours = new();
        Mat m = new();

        CvInvoke.FindContours(temp, contours, m, Emgu.CV.CvEnum.RetrType.External, Emgu.CV.CvEnum.ChainApproxMethod.ChainApproxSimple);


        for (int i = 0; i < contours.Size; i++)
        {
            double perimeter = CvInvoke.ArcLength(contours[i], true);
            VectorOfPoint approx = new();
            CvInvoke.ApproxPolyDP(contours[i], approx, 0.04 * perimeter, true);

            if (CvInvoke.ContourArea(contours[i]) > contourThreshold)
            {
                CvInvoke.DrawContours(currentFrame, contours, i, new MCvScalar(0, 0, 255));

                var moments = CvInvoke.Moments(contours[i]);
                int x = (int)(moments.M10 / moments.M00);
                int y = (int)(moments.M01 / moments.M00);

                if (approx.Size == 3)
                {
                    CvInvoke.PutText(currentFrame, "Triangle", new Point(x, y),
                        Emgu.CV.CvEnum.FontFace.HersheySimplex, 0.5, new MCvScalar(0, 0, 255), 2);
                }
                else if (approx.Size == 4)
                {
                    CvInvoke.PutText(currentFrame, "Rectangle", new Point(x, y),
                        Emgu.CV.CvEnum.FontFace.HersheySimplex, 0.5, new MCvScalar(0, 0, 255), 2);
                }
                else if (approx.Size > 4)
                {
                    CvInvoke.PutText(currentFrame, "Circle", new Point(x, y),
                        Emgu.CV.CvEnum.FontFace.HersheySimplex, 0.5, new MCvScalar(0, 0, 255), 2);
                }
            }

        }

        return currentFrame;
    }

    void DisplayFrameOnPlane()
    {
        if (frame.IsEmpty)
            return;

        Mat currentFrame = ShapeDetection(frame);
        
        CvInvoke.Resize(currentFrame, currentFrame, new Size(t.width, t.height));
        CvInvoke.CvtColor(currentFrame, currentFrame, Emgu.CV.CvEnum.ColorConversion.Bgr2Rgba);
        //CvInvoke.Flip(currentFrame, currentFrame, Emgu.CV.CvEnum.FlipType.Horizontal);
        CvInvoke.Flip(currentFrame, currentFrame, Emgu.CV.CvEnum.FlipType.Vertical);

        t.LoadRawTextureData(currentFrame.DataPointer, currentFrame.Width * currentFrame.Height * 4); // when currentFrame is Mat
        //t.LoadRawTextureData(currentFrame.Ptr, currentFrame.Width * currentFrame.Height * 4);
        t.Apply();
    }

}