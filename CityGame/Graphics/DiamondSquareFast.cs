//Original from: https://gist.github.com/awilki01/83b65ad852a0ab30192af07cda3d7c0b
//Adam Wilkins 

// This code was converted from Java into C#
// The original Java code was found on Stack Overflow by user M. Jessup
// https://stackoverflow.com/questions/2755750/diamond-square-algorithm?newreg=ee2a40d2fe9f49b9b938151e933860d2

using System;

namespace CityGame.Graphics
{
    public class DiamondSquareFast
    {
        private int terrainSize;
        private int roughness;
        private Random rnd = new Random();

        private int[,] t;

        public DiamondSquareFast(int terrainSize, int roughness)
        {
            this.terrainSize = terrainSize;
            this.roughness = roughness;
        
        }

        private void DeomandStep(int x1, int y1, int x2, int y2, int halfSide)
        {
            int x = x1 + halfSide;
            int y = y1 + halfSide;
            t[x, y] = (t[x1, y1] + t[x2, y2] + t[x1, y2] + t[x2, y1] + rnd.Next(512)) / 5;
        }

        private void SquareStep(int x1, int y1, int x2, int y2, int halfSide)
        {
            //sum of square
            int sum = (t[x1, y1] + t[x2, y2] + t[x1, y2] + t[x2, y1] + t[x1 + halfSide, y1 + halfSide]) / 5;
            //point 1
            t[(x2 - x1) / 2, y1] = (sum + rnd.Next(512)) / 2;

            //point 2
            t[x2, (y2 - y1) / 2] = (sum + rnd.Next(512)) / 2;

            //point 3
            t[(x2 - x1) / 2, y2] = (sum + rnd.Next(512)) / 2;

            //point 4
            t[x1, (y2 - y1) / 2] = (sum + rnd.Next(512)) / 2;
        }

        private void DeomandApStep(int x1, int y1, int x2, int y2, int halfSide)
        {
            int x = x1 + halfSide;
            int y = y1 + halfSide;
            t[x, y] = (t[x, y] + t[x1, y1] + t[x2, y2] + t[x1, y2] + t[x2, y1]) / 5;
        }

        private void SquareApStep(int x1, int y1, int x2, int y2, int halfSide)
        {
            //sum of square
            int sum = (t[x1, y1] + t[x2, y2] + t[x1, y2] + t[x2, y1] + t[x1 + halfSide, y1 + halfSide]) / 5;
            //point 1
            t[(x2 - x1) / 2, y1] = (sum + t[(x2 - x1) / 2, y1]) / 2;

            //point 2
            t[x2, (y2 - y1) / 2] = (sum + t[x2, (y2 - y1) / 2]) / 2;

            //point 3
            t[(x2 - x1) / 2, y2] = (sum + t[(x2 - x1) / 2, y2]) / 2;

            //point 4
            t[x1, (y2 - y1) / 2] = (sum + t[x1, (y2 - y1) / 2]) / 2;
        }


        public int[,] getData()
        {
            t = new int[terrainSize, terrainSize]; //"t" mean terrain
            //Init 
            t[0, 0] = rnd.Next(512);
            t[terrainSize - 1, 0] = rnd.Next(512);
            t[terrainSize - 1, terrainSize - 1] = rnd.Next(512);
            t[0, terrainSize - 1] = rnd.Next(512);

            //Steps            
            int sideLength = terrainSize - 1;
            int sum = 0;
            while (true)
            {

                int halfSide = (int)Math.Ceiling(sideLength / 2.0f);
                for (int x = 0; x < terrainSize - 1; x += sideLength)
                {
                    for (int y = 0; y < terrainSize - 1; y += sideLength)
                    {
                            //DeomandStep(x, y, x + sideLength, y + sideLength, halfSide);
                            if ((x + halfSide < terrainSize) && (y + halfSide < terrainSize)
                                && (x + sideLength < terrainSize) && (y + sideLength < terrainSize))
                            {
                                t[x + halfSide, y + halfSide] = (t[x, y] + t[x + sideLength, y + sideLength] + t[x, y + sideLength] + t[x + sideLength, y] + rnd.Next(512)) / 5;


                                //SquareStep(x, y, x + sideLength, y + sideLength, halfSide);
                                //^^^this generate flat terrain
                                //sum of square

                                //NOTE: this generate terrain with sea on righ buttom terrain side
                                sum = (t[x, y] + t[x + sideLength, y + sideLength] + t[x, y + sideLength] + t[x + sideLength, y] + t[x + halfSide, y + halfSide]) / 5;
                                //point 1
                                t[(x + sideLength) / 2, y] = (sum + rnd.Next(512)) / 2;

                                //point 2
                                t[x + sideLength, (y + sideLength) / 2] = (sum + rnd.Next(512)) / 2;

                                //point 3
                                t[(x + sideLength) / 2, y + sideLength] = (sum + rnd.Next(512)) / 2;

                                //point 4
                                t[x, (y + sideLength) / 2] = (sum + rnd.Next(512)) / 2;
                            }
                            
                        
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
                            //DeomandStep(x, y, x + sideLength, y + sideLength, halfSide);
                            if ((x + halfSide < terrainSize) && (y + halfSide < terrainSize)
                                && (x + sideLength < terrainSize) && (y + sideLength < terrainSize))
                            {
                                t[x + halfSide, y + halfSide] = (t[x, y] + t[x + sideLength, y + sideLength] + t[x, y + sideLength] + t[x + sideLength, y] + t[x + halfSide, y + halfSide]) / 5;


                                //SquareStep(x, y, x + sideLength, y + sideLength, halfSide);
                                //^^^this generate flat terrain
                                //sum of square

                                //NOTE: this generate terrain with sea on righ buttom terrain side
                                sum = (t[x, y] + t[x + sideLength, y + sideLength] + t[x, y + sideLength] + t[x + sideLength, y] + t[x + halfSide, y + halfSide]) / 5;
                                //point 1
                                t[(x + sideLength) / 2, y] = (sum + t[(x + sideLength) / 2, y]) / 2;

                                //point 2
                                t[x + sideLength, (y + sideLength) / 2] = (sum + t[x + sideLength, (y + sideLength) / 2]) / 2;

                                //point 3
                                t[(x + sideLength) / 2, y + sideLength] = (sum + t[(x + sideLength) / 2, y + sideLength]) / 2;

                                //point 4
                                t[x, (y + sideLength) / 2] = (sum + t[x, (y + sideLength) / 2]) / 2;
                            }
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
