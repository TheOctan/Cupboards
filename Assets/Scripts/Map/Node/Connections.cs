namespace OctanGames.Map.Node
{
    public struct Connections
    {
        public readonly int U;
        public readonly int R;
        public readonly int D;
        public readonly int L;

        public Connections(int u, int r, int d, int l)
        {
            L = l;
            D = d;
            R = r;
            U = u;
        }
    }
}