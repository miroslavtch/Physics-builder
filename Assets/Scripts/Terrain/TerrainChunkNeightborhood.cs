   public class TerrainChunkNeighborhood
    {
        public Chunk XUp { get; set; }

        public Chunk XDown { get; set; }

        public Chunk ZUp { get; set; }

        public Chunk ZDown { get; set; }
    }

    public enum TerrainNeighbor
    {
        XUp,
        XDown,
        ZUp,
        ZDown
    }