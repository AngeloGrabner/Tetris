public enum Shape
{
    I_0 = 0,
    I_90 = 1,
    I_180 = 2,
    I_270 = 3,

    O_0 = 4,
    O_90 = 5,
    O_180 = 6,
    O_270 = 7,

    L_0 = 8,
    L_90 = 9,
    L_180 = 10,
    L_270 = 11,

    J_0 = 12,
    J_90 = 13,
    J_180 = 14,
    J_270 = 15,

    T_0 = 16,
    T_90 = 17,
    T_180 = 18,
    T_270 = 19,

    S_0 = 20,
    S_90 = 21,
    S_180 = 22,
    S_270 = 23,

    Z_0 = 24,
    Z_90 = 25,
    Z_180 = 26,
    Z_270 = 27,
}

public class Tile
{
    public readonly char endCharacter;
    public int X;
    public int Y;
    private char[,] _map = new char[4, 4];
    public char[,] map
    {
        get { return _map; }
    }
    public Shape shape;
    public Tile(int X, int Y, Shape shape)
    {
        this.X = X;
        this.Y = Y;
        this.shape = shape;
        fillMap(shape);
        endCharacter = getEndCharacter(shape);
    }
    private char getEndCharacter(Shape s)
    {
        switch (s)
        {
            case Shape.I_0:
            case Shape.I_90:
            case Shape.I_180:
            case Shape.I_270:
                return 'I';
            case Shape.J_0:
            case Shape.J_90:
            case Shape.J_180:
            case Shape.J_270:
                return 'J';
            case Shape.L_0:
            case Shape.L_90:
            case Shape.L_180:
            case Shape.L_270:
                return 'L';
            case Shape.O_0:
            case Shape.O_90:
            case Shape.O_180:
            case Shape.O_270:
                return 'O';
            case Shape.S_0:
            case Shape.S_90:
            case Shape.S_180:
            case Shape.S_270:
                return 'S';
            case Shape.T_0:
            case Shape.T_90:
            case Shape.T_180:
            case Shape.T_270:
                return 'T';
            case Shape.Z_0:
            case Shape.Z_90:
            case Shape.Z_180:
            case Shape.Z_270:
                return 'Z';
        }
        return '#';
    }
    public void fillMap(Shape s)
    {
        switch (s)
        {
            case Shape.I_0:
                _map = new char[,]
                { {' ', 'X', ' ', ' ' },
                  {' ', 'X', ' ', ' ' },
                  {' ', 'X', ' ', ' ' },
                  {' ', 'X', ' ', ' ' } };
                break;
            case Shape.I_90:
                _map = new char[,]
                { {' ', ' ', ' ', ' ' },
                  {'X', 'X', 'X', 'X' },
                  {' ', ' ', ' ', ' ' },
                  {' ', ' ', ' ', ' ' } };
                break;
            case Shape.I_180:
                _map = new char[,]
                { {' ', ' ', 'X', ' ' },
                  {' ', ' ', 'X', ' ' },
                  {' ', ' ', 'X', ' ' },
                  {' ', ' ', 'X', ' ' } };
                break;
            case Shape.I_270:
                _map = new char[,]
                { {' ', ' ', ' ', ' ' },
                  {' ', ' ', ' ', ' ' },
                  {'X', 'X', 'X', 'X' },
                  {' ', ' ', ' ', ' ' } };
                break;
            case Shape.O_0:
            case Shape.O_90:
            case Shape.O_180:
            case Shape.O_270:
                _map = new char[,]
                { {' ', ' ', ' ', ' ' },
                  {' ', 'X', 'X', ' ' },
                  {' ', 'X', 'X', ' ' },
                  {' ', ' ', ' ', ' ' } };
                break;
            case Shape.L_0:
                _map = new char[,]
                { {' ', ' ', ' ', ' ' },
                  {' ', 'X', ' ', ' ' },
                  {' ', 'X', ' ', ' ' },
                  {' ', 'X', 'X', ' ' } };
                break;
            case Shape.L_90:
                _map = new char[,]
                { {' ', ' ', ' ', ' ' },
                  {'X', 'X', 'X', ' ' },
                  {'X', ' ', ' ', ' ' },
                  {' ', ' ', ' ', ' ' } };
                break;
            case Shape.L_180:
                _map = new char[,]
                { {' ', 'X', 'X', ' ' },
                  {' ', ' ', 'X', ' ' },
                  {' ', ' ', 'X', ' ' },
                  {' ', ' ', ' ', ' ' } };
                break;
            case Shape.L_270:
                _map = new char[,]
                { {' ', ' ', ' ', ' ' },
                  {' ', ' ', ' ', 'X' },
                  {' ', 'X', 'X', 'X' },
                  {' ', ' ', ' ', ' ' } };
                break;
            case Shape.J_0:
                _map = new char[,]
                { {' ', ' ', ' ', ' ' },
                  {' ', ' ', 'X', ' ' },
                  {' ', ' ', 'X', ' ' },
                  {' ', 'X', 'X', ' ' } };
                break;
            case Shape.J_90:
                _map = new char[,]
                { {' ', ' ', ' ', ' ' },
                  {'X', ' ', ' ', ' ' },
                  {'X', 'X', 'X', ' ' },
                  {' ', ' ', ' ', ' ' } };
                break;
            case Shape.J_180:
                _map = new char[,]
                { {' ', 'X', 'X', ' ' },
                  {' ', 'X', ' ', ' ' },
                  {' ', 'X', ' ', ' ' },
                  {' ', ' ', ' ', ' ' } };
                break;
            case Shape.J_270:
                _map = new char[,]
                { {' ', ' ', ' ', ' ' },
                  {' ', 'X', 'X', 'X' },
                  {' ', ' ', ' ', 'X' },
                  {' ', ' ', ' ', ' ' } };
                break;
            case Shape.T_0:
                _map = new char[,]
                { {' ', ' ', ' ', ' ' },
                  {'X', 'X', 'X', ' ' },
                  {' ', 'X', ' ', ' ' },
                  {' ', ' ', ' ', ' ' } };
                break;
            case Shape.T_90:
                _map = new char[,]
                { {' ', 'X', ' ', ' ' },
                  {'X', 'X', ' ', ' ' },
                  {' ', 'X', ' ', ' ' },
                  {' ', ' ', ' ', ' ' } };
                break;
            case Shape.T_180:
                _map = new char[,]
                { {' ', 'X', ' ', ' ' },
                  {'X', 'X', 'X', ' ' },
                  {' ', ' ', ' ', ' ' },
                  {' ', ' ', ' ', ' ' } };
                break;
            case Shape.T_270:
                _map = new char[,]
                { {' ', 'X', ' ', ' ' },
                  {' ', 'X', 'X', ' ' },
                  {' ', 'X', ' ', ' ' },
                  {' ', ' ', ' ', ' ' } };
                break;
            case Shape.S_0:
            case Shape.S_180:
                _map = new char[,]
                { {' ', ' ', ' ', ' ' },
                  {' ', 'X', 'X', ' ' },
                  {'X', 'X', ' ', ' ' },
                  {' ', ' ', ' ', ' ' } };
                break;
            case Shape.S_90:
            case Shape.S_270:
                _map = new char[,]
                { {' ', 'X', ' ', ' ' },
                  {' ', 'X', 'X', ' ' },
                  {' ', ' ', 'X', ' ' },
                  {' ', ' ', ' ', ' ' } };
                break;
            case Shape.Z_0:
            case Shape.Z_180:
                _map = new char[,]
                { {' ', ' ', ' ', ' ' },
                  {'X', 'X', ' ', ' ' },
                  {' ', 'X', 'X', ' ' },
                  {' ', ' ', ' ', ' ' } };
                break;
            case Shape.Z_90:
            case Shape.Z_270:
                _map = new char[,]
                { {' ', 'X', ' ', ' ' },
                  {'X', 'X', ' ', ' ' },
                  {'X', ' ', ' ', ' ' },
                  {' ', ' ', ' ', ' ' } };
                break;
            default:
                break;
        }
    }
}
