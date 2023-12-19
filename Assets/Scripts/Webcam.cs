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
    [SerializeField] float minContourThreshold, maxContourThreshold;
    [SerializeField, Range(0, 25)] int erode, dilate;
    [SerializeField] RawImage otherImage;
    //[SerializeField] bool Colorized;
    VideoCapture capture;
    Mat frame;
    Texture2D tColorized, tGray;

    public string maskShown;

    RawImage webcamScreen;

    void Start()
    {
        webcamScreen = gameObject.GetComponent<RawImage>();

        frame = new();
        capture = new();
        capture.ImageGrabbed += HandleWebcamQueryFrame;

        tColorized = new Texture2D((int)webcamScreen.rectTransform.rect.width, (int)webcamScreen.rectTransform.rect.height, TextureFormat.RGBA32, false);
        gameObject.GetComponent<RawImage>().texture = tColorized;
        
        tGray = new Texture2D((int)webcamScreen.rectTransform.rect.width, (int)webcamScreen.rectTransform.rect.height, TextureFormat.RGBA32, false);
        otherImage.texture = tGray;

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

    Image<Gray, byte> ColorDetection(Mat frame, Bgr lower, Bgr higher)
    {
        return frame.ToImage<Bgr, byte>().InRange(lower, higher);
    }

    Mat ShapeDetection(Mat frame)
    {
        Bgr lower = new Bgr(100, 0, 0);
        Bgr higher = new Bgr(255, 100, 100);
        var maskBlue = ColorDetection(frame, lower, higher);
        maskBlue._Erode(erode);
        maskBlue._Dilate(dilate);

        lower = new Bgr(0, 80, 0);
        higher = new Bgr(80, 255, 80);
        var maskGreen = ColorDetection(frame, lower, higher);
        maskGreen._Erode(erode);
        maskGreen._Dilate(dilate);

        lower = new Bgr(0, 0, 125);
        higher = new Bgr(80, 100, 255);
        var maskRed = ColorDetection(frame, lower, higher);
        maskRed._Erode(erode);
        maskRed._Dilate(dilate);


        Dictionary<string, Image<Gray, byte>> masks = new();
        masks.Add("Blue", maskBlue);
        masks.Add("Green", maskGreen);
        masks.Add("Red", maskRed);

        Dictionary<string, int> results = new();
        Mat currentFrame = (maskBlue + maskGreen + maskRed).Convert<Bgr, byte>().Mat;

        foreach (var item in masks)
        {
            var temp = item.Value;
            VectorOfVectorOfPoint contours = new();
            Mat m = new();

            CvInvoke.FindContours(temp, contours, m, Emgu.CV.CvEnum.RetrType.External, Emgu.CV.CvEnum.ChainApproxMethod.ChainApproxSimple);

            for (int i = 0; i < contours.Size; i++)
            {
                double perimeter = CvInvoke.ArcLength(contours[i], true);
                VectorOfPoint approx = new();
                CvInvoke.ApproxPolyDP(contours[i], approx, 0.04 * perimeter, true);

                if (CvInvoke.ContourArea(contours[i]) > minContourThreshold &&
                    CvInvoke.ContourArea(contours[i]) < maxContourThreshold)
                {
                    CvInvoke.DrawContours(currentFrame, contours, i, new MCvScalar(0, 0, 255));

                    var moments = CvInvoke.Moments(contours[i]);
                    int x = (int)(moments.M10 / moments.M00);
                    int y = (int)(moments.M01 / moments.M00);

                    string keyName = item.Key;
                    if (approx.Size == 3)
                    {
                        keyName += "Triangle";
                        if (results.ContainsKey(keyName))
                        {
                            results[keyName]++;
                        }
                        else
                        {
                            results.Add(keyName, 1);
                        }
                        CvInvoke.PutText(currentFrame, "Triangle", new Point(x, y),
                            Emgu.CV.CvEnum.FontFace.HersheySimplex, 0.5, new MCvScalar(0, 0, 255), 2);
                    }
                    else if (approx.Size == 4)
                    {
                        keyName += "Rectangle";
                        if (results.ContainsKey(keyName))
                        {
                            results[keyName]++;
                        }
                        else
                        {
                            results.Add(keyName, 1);
                        }
                        CvInvoke.PutText(currentFrame, "Rectangle", new Point(x, y),
                            Emgu.CV.CvEnum.FontFace.HersheySimplex, 0.5, new MCvScalar(0, 0, 255), 2);
                    }
                    else if (approx.Size > 4)
                    {
                        keyName += "Circle";
                        if (results.ContainsKey(keyName))
                        {
                            results[keyName]++;
                        }
                        else
                        {
                            results.Add(keyName, 1);
                        }
                        CvInvoke.PutText(currentFrame, "Circle", new Point(x, y),
                            Emgu.CV.CvEnum.FontFace.HersheySimplex, 0.5, new MCvScalar(0, 0, 255), 2);
                    }
                }
            }

        }

        GameManager.Instance.UpdateShapeCount(
            results.GetValueOrDefault("RedCircle"), 
            results.GetValueOrDefault("BlueRectangle"),
            results.GetValueOrDefault("GreenTriangle")
        );

        if (masks.TryGetValue(maskShown, out var value))
            return value.Mat;
        return currentFrame;
    }

    void DisplayFrameOnPlane()
    {
        if (frame.IsEmpty)
            return;

        Mat frameGray = ShapeDetection(frame);
        Mat frameColorized = frame.Clone();
        
        CvInvoke.Resize(frameGray, frameGray, new Size(tColorized.width, tColorized.height));
        CvInvoke.CvtColor(frameGray, frameGray, Emgu.CV.CvEnum.ColorConversion.Bgr2Rgba);
        CvInvoke.Flip(frameGray, frameGray, Emgu.CV.CvEnum.FlipType.Vertical);

        tGray.LoadRawTextureData(frameGray.DataPointer, frameGray.Width * frameGray.Height * 4); 
        tGray.Apply();


        CvInvoke.Resize(frameColorized, frameColorized, new Size(tColorized.width, tColorized.height));
        CvInvoke.CvtColor(frameColorized, frameColorized, Emgu.CV.CvEnum.ColorConversion.Bgr2Rgba);
        CvInvoke.Flip(frameColorized, frameColorized, Emgu.CV.CvEnum.FlipType.Vertical);

        tColorized.LoadRawTextureData(frameColorized.DataPointer, frameColorized.Width * frameColorized.Height * 4);
        tColorized.Apply();
    }

}