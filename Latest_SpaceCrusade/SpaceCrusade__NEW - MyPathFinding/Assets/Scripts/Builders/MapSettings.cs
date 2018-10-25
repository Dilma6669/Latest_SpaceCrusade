using UnityEngine;

public class MapSettings : MonoBehaviour {

    private int _worldSizeX = 10;
    public int worldSizeX { get { return _worldSizeX; } set { _worldSizeX = value; } }

    private int _worldSizeZ = 10;
    public int worldSizeZ { get { return _worldSizeZ; } set { _worldSizeZ = value; } }

    private int _worldSizeY = 45;
    public int worldSizeY { get { return _worldSizeY; } set { _worldSizeY = value; } }

    private int _worldType = 0; // 0 = square, 1 = Line, 2 = tower
    public int worldType { get { return _worldType; } set { _worldType = value; } }

    //////////////////////////////

    private int _sizeOfMapPiecesXZ = 24; // 24
    public int sizeOfMapPiecesXZ { get { return _sizeOfMapPiecesXZ; } set { _sizeOfMapPiecesXZ = value; } }

    private int _sizeOfMapPiecesY = 6; // 6
    public int sizeOfMapPiecesY { get { return _sizeOfMapPiecesY; } set { _sizeOfMapPiecesY = value; } }

    private int _sizeOfMapVentsY = 2; // 2
    public int sizeOfMapVentsY { get { return _sizeOfMapVentsY; } set { _sizeOfMapVentsY = value; } }




    private int _worldNodeDistanceXZ = 2; // 1 less than max map size. Space inbetween nodes. needs a +1 to get new location
    public int worldNodeDistanceXZ { get { return _worldNodeDistanceXZ; } set { _worldNodeDistanceXZ = value; } }

    private int _worldNodeDistanceY = 2; // Space inbetween nodes. needs a +1 to get new location
    public int worldNodeDistanceY { get { return _worldNodeDistanceY; } set { _worldNodeDistanceY = value; } }




    private int _worldPadding = -20; // 10 * 24 = nodes start at X : 240
    public int worldPadding { get { return _worldPadding; } set { _worldPadding = value; } }

    private int _sizeOfCubes = 1; // 1
    public int sizeOfCube { get { return _sizeOfCubes; } set { _sizeOfCubes = value; } }

    private int[] sizes;
    public int getRandomMapSize { get { return sizes[Random.Range(0, sizes.Length)]; } }

    private int _sizeOfMapConnectorsXYZ = 1; // 1
    public int sizeOfMapConnectorsXYZ { get { return _sizeOfMapConnectorsXYZ; } set { _sizeOfMapConnectorsXYZ = value; } }


    void Awake() {
        sizes = new int[] { 1, 3 };
    }


}
