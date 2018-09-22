
public class WorldNode : BaseNode
{
    public int worldNodeCount;
    public bool connected; // Used to determine if a node has had its neighbours defined
    public int[] neighbours;

    void Awake()
    {
        thisNodeType = this;
    }

}
