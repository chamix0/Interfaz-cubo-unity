namespace BluetoothCubo
{
    public class CubeTracker
    {
        private string lastMove;

        public CubeTracker()
        {
            lastMove = null;
        }

        public string ReadFaces(string move)
        {
            string face = "";
            string directionSimbol = "";
            string firstHalf = move.Substring(0, 4);
            string secondHalf = move.Substring(4, 4);

            switch (firstHalf)
            {
                case "0101":
                    face = "R";
                    break;
                case "0011":
                    face = "L";
                    break;
                case "0001":
                    face = "F";
                    break;
                case "0110":
                    face = "B";
                    break;
                case "0010":
                    face = "U";
                    break;
                case "0100":
                    face = "D";
                    break;
            }

            if (secondHalf == "0011")
            {
                directionSimbol = "\u0027";
                face += "0";
            }

            lastMove = face;
            return "" + face + directionSimbol;
        }
    }
}