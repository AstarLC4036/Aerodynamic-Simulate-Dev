using BA.Flight;
using BA.FlightPhysics;
using Flight;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace BA.CameraUtil
{
    [ExecuteInEditMode]
    public class AeroVisual : MonoBehaviour
    {
        public Aircraft targetAircraft;
        public AeroSurface targetSurface;

        public bool c_disp_Enable = false;
        public bool c_disp_HoriMode = false;
        public int c_disp_StepSize = 1;
        public float c_disp_Spacing = 1;
        public Vector2 c_disp_Offset = Vector2.zero;
        public Vector2 c_disp_Scale = Vector3.one;

        [SerializeField]
        private Material gl_mat;

        private void OnEnable()
        {
            RenderPipelineManager.endCameraRendering += OnPostRenderURP;
        }

        private void OnDisable()
        {
            RenderPipelineManager.endCameraRendering -= OnPostRenderURP;
        }

        private void OnPostRenderURP(ScriptableRenderContext arg1, Camera arg2)
        {
            if (c_disp_Enable)
            {
                float ox = 0;
                float oy = 0;
                if (c_disp_HoriMode)
                {
                    ox = c_disp_Spacing;
                }
                else
                {
                    oy = c_disp_Spacing;
                }

                Vector2[] pointsL = new Vector2[360 / c_disp_StepSize];
                Vector2[] pointsD = new Vector2[360 / c_disp_StepSize];
                Vector2[] pointsT = new Vector2[360 / c_disp_StepSize];

                for (int i = -180; i < 180; i += c_disp_StepSize)
                {
                    LDMCoefficients coefficients = targetSurface.CalcucateCoiffients(i);
                    float coeL = coefficients.liftCoefficient;
                    float coeD = coefficients.dragCoefficient;
                    float coeT = coefficients.torqueCoefficient;

                    pointsL[i + 180] = new Vector2(i, coeL);
                    pointsD[i + 180] = new Vector2(i, coeD);
                    pointsT[i + 180] = new Vector2(i, coeT);
                }

                GL.PushMatrix();
                gl_mat.SetPass(0);
                GL.LoadOrtho();
                GL.Begin(GL.LINES);
                GL.Color(Color.white);

                //坐标轴 L
                GL.Vertex3(c_disp_Offset.x - 180 * c_disp_Scale.x, c_disp_Offset.y, 0);
                GL.Vertex3(c_disp_Offset.x + 180 * c_disp_Scale.x, c_disp_Offset.y, 0);
                GL.Vertex3(c_disp_Offset.x, c_disp_Offset.y - 2 * c_disp_Scale.y, 0);
                GL.Vertex3(c_disp_Offset.x, c_disp_Offset.y + 2 * c_disp_Scale.y, 0);

                //单位1
                GL.Vertex3(c_disp_Offset.x - 0.01f, c_disp_Offset.y + c_disp_Scale.y, 0);
                GL.Vertex3(c_disp_Offset.x + 0.01f, c_disp_Offset.y + c_disp_Scale.y, 0);
                GL.Vertex3(c_disp_Offset.x - 0.01f, c_disp_Offset.y - c_disp_Scale.y, 0);
                GL.Vertex3(c_disp_Offset.x + 0.01f, c_disp_Offset.y - c_disp_Scale.y, 0);

                //迎角
                GL.Vertex3(c_disp_Offset.x + targetAircraft.AOA * c_disp_Scale.x, c_disp_Offset.y - 2 * c_disp_Scale.y, 0);
                GL.Vertex3(c_disp_Offset.x + targetAircraft.AOA * c_disp_Scale.x, c_disp_Offset.y + 2 * c_disp_Scale.y, 0);

                //坐标轴 D
                GL.Vertex3(c_disp_Offset.x - 180 * c_disp_Scale.x + ox, c_disp_Offset.y + oy, 0);
                GL.Vertex3(c_disp_Offset.x + 180 * c_disp_Scale.x + ox, c_disp_Offset.y + oy, 0);
                GL.Vertex3(c_disp_Offset.x + ox, c_disp_Offset.y - 2 * c_disp_Scale.y + oy, 0);
                GL.Vertex3(c_disp_Offset.x + ox, c_disp_Offset.y + 2 * c_disp_Scale.y + oy, 0);

                //单位1
                GL.Vertex3(c_disp_Offset.x - 0.01f + ox, c_disp_Offset.y + c_disp_Scale.y + oy, 0);
                GL.Vertex3(c_disp_Offset.x + 0.01f + ox, c_disp_Offset.y + c_disp_Scale.y + oy, 0);
                GL.Vertex3(c_disp_Offset.x - 0.01f + ox, c_disp_Offset.y - c_disp_Scale.y + oy, 0);
                GL.Vertex3(c_disp_Offset.x + 0.01f + ox, c_disp_Offset.y - c_disp_Scale.y + oy, 0);

                //迎角
                GL.Vertex3(c_disp_Offset.x + ox + targetAircraft.AOA * c_disp_Scale.x, c_disp_Offset.y - 2 * c_disp_Scale.y + oy, 0);
                GL.Vertex3(c_disp_Offset.x + ox + targetAircraft.AOA * c_disp_Scale.x, c_disp_Offset.y + 2 * c_disp_Scale.y + oy, 0);

                //坐标轴 T
                GL.Vertex3(c_disp_Offset.x - 180 * c_disp_Scale.x + ox * 2, c_disp_Offset.y + oy * 2, 0);
                GL.Vertex3(c_disp_Offset.x + 180 * c_disp_Scale.x + ox * 2, c_disp_Offset.y + oy * 2, 0);
                GL.Vertex3(c_disp_Offset.x + ox * 2, c_disp_Offset.y - 2 * c_disp_Scale.y + oy * 2, 0);
                GL.Vertex3(c_disp_Offset.x + ox * 2, c_disp_Offset.y + 2 * c_disp_Scale.y + oy * 2, 0);

                //单位1
                GL.Vertex3(c_disp_Offset.x - 0.01f + ox * 2, c_disp_Offset.y + c_disp_Scale.y + oy * 2, 0);
                GL.Vertex3(c_disp_Offset.x + 0.01f + ox * 2, c_disp_Offset.y + c_disp_Scale.y + oy * 2, 0);
                GL.Vertex3(c_disp_Offset.x - 0.01f + ox * 2, c_disp_Offset.y - c_disp_Scale.y + oy * 2, 0);
                GL.Vertex3(c_disp_Offset.x + 0.01f + ox * 2, c_disp_Offset.y - c_disp_Scale.y + oy * 2, 0);

                //迎角
                GL.Vertex3(c_disp_Offset.x + ox * 2 + targetAircraft.AOA * c_disp_Scale.x, c_disp_Offset.y - 2 * c_disp_Scale.y + oy * 2, 0);
                GL.Vertex3(c_disp_Offset.x + ox * 2 + targetAircraft.AOA * c_disp_Scale.x, c_disp_Offset.y + 2 * c_disp_Scale.y + oy * 2, 0);

                GL.End();
                GL.PopMatrix();


                GL.PushMatrix();
                gl_mat.SetPass(0);
                GL.LoadOrtho();
                GL.Begin(GL.LINES);
                GL.Color(Color.white);

                //GL.Color(Color.green);

                for (int i = 0; i < pointsL.Length - 1; i++)
                {
                    if (i == pointsL.Length - 1)
                        continue;

                    Vector2 nextPoint = pointsL[i + 1];

                    Vector2 point = pointsL[i];

                    GL.Vertex3(c_disp_Offset.x + point.x * c_disp_Scale.x, c_disp_Offset.y + point.y * c_disp_Scale.y, 0);
                    GL.Vertex3(c_disp_Offset.x + nextPoint.x * c_disp_Scale.x, c_disp_Offset.y + nextPoint.y * c_disp_Scale.y, 0);
                }

                for (int i = 0; i < pointsD.Length - 1; i++)
                {
                    if (i == pointsD.Length - 1)
                        continue;

                    Vector2 nextPoint = pointsD[i + 1];

                    Vector2 point = pointsD[i];

                    GL.Vertex3(c_disp_Offset.x + point.x * c_disp_Scale.x + ox, c_disp_Offset.y + point.y * c_disp_Scale.y + oy, 0);
                    GL.Vertex3(c_disp_Offset.x + nextPoint.x * c_disp_Scale.x + ox, c_disp_Offset.y + nextPoint.y * c_disp_Scale.y + oy, 0);
                }

                for (int i = 0; i < pointsT.Length - 1; i++)
                {
                    if (i == pointsT.Length - 1)
                        continue;

                    Vector2 nextPoint = pointsT[i + 1];

                    Vector2 point = pointsT[i];

                    GL.Vertex3(c_disp_Offset.x + point.x * c_disp_Scale.x + ox * 2, c_disp_Offset.y + point.y * c_disp_Scale.y + oy * 2, 0);
                    GL.Vertex3(c_disp_Offset.x + nextPoint.x * c_disp_Scale.x + ox * 2, c_disp_Offset.y + nextPoint.y * c_disp_Scale.y + oy * 2, 0);
                }

                GL.End();
                GL.PopMatrix();
            }
        }
    }
}
