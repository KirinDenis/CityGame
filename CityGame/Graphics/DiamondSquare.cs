//Original from: https://gist.github.com/awilki01/83b65ad852a0ab30192af07cda3d7c0b
//Adam Wilkins 

// This code was converted from Java into C#
// The original Java code was found on Stack Overflow by user M. Jessup
// https://stackoverflow.com/questions/2755750/diamond-square-algorithm?newreg=ee2a40d2fe9f49b9b938151e933860d2

using System;

namespace CityGame.Graphics
{
    public class DiamondSquare
    {
        private int terrainSize;
        private int roughness;
        private Random rnd = new Random();
        private double[,] t;

        public DiamondSquare(int terrainSize, int roughness)
        {
            this.terrainSize = terrainSize;
            this.roughness = roughness;
        }

        private void DeomandStep(int x1, int y1, int x2, int y2, int halfSide)
        {
            int x = x1 + halfSide;
            int y = y1 + halfSide;
            t[x, y] = (t[x1, y1] + t[x2, y2] + t[x1, y2] + t[x2, y1] + rnd.NextDouble()) / 5.0f;
        }

        private void SquareStep(int x1, int y1, int x2, int y2, int halfSide)
        {
            //sum of square
            double sum = (t[x1, y1] + t[x2, y2] + t[x1, y2] + t[x2, y1] + t[x1 + halfSide, y1 + halfSide]) / 5.0f;
            //point 1
            t[(x2 - x1) / 2, y1] = (sum + rnd.NextDouble()) / 2.0f;

            //point 2
            t[x2, (y2 - y1) / 2] = (sum + rnd.NextDouble()) / 2.0f;

            //point 3
            t[(x2 - x1) / 2, y2] = (sum + rnd.NextDouble()) / 2.0f;

            //point 4
            t[x1, (y2 - y1) / 2] = (sum + rnd.NextDouble()) / 2.0f;
        }

        private void DeomandApStep(int x1, int y1, int x2, int y2, int halfSide)
        {
            int x = x1 + halfSide;
            int y = y1 + halfSide;
            t[x, y] = (t[x, y] + t[x1, y1] + t[x2, y2] + t[x1, y2] + t[x2, y1]) / 5.0f;
        }

        private void SquareApStep(int x1, int y1, int x2, int y2, int halfSide)
        {
            //sum of square
            double sum = (t[x1, y1] + t[x2, y2] + t[x1, y2] + t[x2, y1] + t[x1 + halfSide, y1 + halfSide]) / 5.0f;
            //point 1
            t[(x2 - x1) / 2, y1] = (sum + t[(x2 - x1) / 2, y1]) / 2.0f;

            //point 2
            t[x2, (y2 - y1) / 2] = (sum + t[x2, (y2 - y1) / 2]) / 2.0f;

            //point 3
            t[(x2 - x1) / 2, y2] = (sum + t[(x2 - x1) / 2, y2]) / 2.0f;

            //point 4
            t[x1, (y2 - y1) / 2] = (sum + t[x1, (y2 - y1) / 2]) / 2.0f;
        }


        public double[,] getData()
        {
            t = new double[terrainSize, terrainSize]; //"t" mean terrain
            //Init 
            t[0, 0] = rnd.NextDouble();
            t[terrainSize - 1, 0] = rnd.NextDouble();
            t[terrainSize - 1, terrainSize - 1] = rnd.NextDouble();
            t[0, terrainSize - 1] = rnd.NextDouble();

            //Steps            
            int sideLength = terrainSize - 1;
            double sum = 0.0f;
            while (true)
            {

                int halfSide = (int)Math.Ceiling(sideLength / 2.0f);
                for (int x = 0; x < terrainSize - 1; x += sideLength)
                {
                    for (int y = 0; y < terrainSize - 1; y += sideLength)
                    {
                        try
                        {
                            //DeomandStep(x, y, x + sideLength, y + sideLength, halfSide);
                            t[x + halfSide, y + halfSide] = (t[x, y] + t[x + sideLength, y + sideLength] + t[x, y + sideLength] + t[x + sideLength, y] + rnd.NextDouble()) / 5.0f;


                            //SquareStep(x, y, x + sideLength, y + sideLength, halfSide);
                            //^^^this generate flat terrain
                            //sum of square
                            
                            //NOTE: this generate terrain with sea on righ buttom terrain side
                            sum = (t[x, y] + t[x + sideLength, y + sideLength] + t[x, y + sideLength] + t[x + sideLength, y] + t[x + halfSide, y + halfSide]) / 5.0f;
                            //point 1
                            t[(x + sideLength) /2, y] = (sum + rnd.NextDouble()) / 2.0f;

                            //point 2
                            t[x + sideLength, (y + sideLength) / 2] = (sum + rnd.NextDouble()) / 2.0f;

                            //point 3
                            t[(x + sideLength) / 2, y + sideLength] = (sum + rnd.NextDouble()) / 2.0f;

                            //point 4
                            t[x, (y + sideLength) / 2] = (sum + rnd.NextDouble()) / 2.0f;
                            
                        }
                        catch
                        {

                        };
                    }
                }
                if (sideLength < 2)
                {
                    break;
                }
                sideLength = (int)Math.Ceiling(sideLength / 2.0f);
            }

            //smooth out roughness
            for (int r = 0; r < roughness; r++)
            {
                sideLength = terrainSize - 1;

                while (true)
                {
                    int halfSide = (int)Math.Ceiling(sideLength / 2.0f);
                    for (int x = 0; x < terrainSize - 1; x += sideLength)
                    {
                        for (int y = 0; y < terrainSize - 1; y += sideLength)
                        {
                            try
                            {
                                DeomandApStep(x, y, x + sideLength, y + sideLength, halfSide);
                                SquareApStep(x, y, x + sideLength, y + sideLength, halfSide);
                            }
                            catch
                            {

                            };
                        }
                    }
                    if (sideLength < 2)
                    {
                        break;
                    }
                    sideLength = (int)Math.Ceiling(sideLength / 2.0f);
                }
            }
            return t;
        }

    }
}
