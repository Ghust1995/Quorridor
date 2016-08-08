using System;
using System.Collections.Generic;
using Logging;

namespace GameData
{
    public class GridMap
    {
        // Number is a pair player-objective, that is two points that must be connectable
        // Everything of the same number must be connected
        // Alphabet is stuff to put in the map.
        // . is empty
        // # is wall
        // % is Reserved
        // may add more stuff

        private const char WALL = '#';
        private const char EMPTY = '.';
        private const char RESERVED = '%';

        //Currently the map is: (hardcoded)

        //#################
        //#0......####%..1#
        //#0......#.1.%..1#
        //#0......####%..1#
        //#################

        //TODO: Read from file might be interesting

        private char[,] _map;

        private Dictionary<char, Tuple<int, int>> _players;

        private int _wallSize;

        /// <summary>
        /// Creates a new gridmap:
        ///#################
        ///#0.............1#
        ///#0........1....1#
        ///#0.............1#
        ///#################
        /// </summary>
        /// <param name="sizeX">The horizontal size of the map</param>
        /// <param name="sizeZ">the vertical size of the map</param>
        /// <param name="wallSize">The wall size (if even will be added 1)</param>
        public GridMap(int sizeX, int sizeZ, int wallSize)
        {
            _map = new char[sizeX, sizeZ];
            _players = new Dictionary<char, Tuple<int, int>>();

            if (wallSize%2 == 0)
            {
                wallSize += 1;
                LoggerSystem.Log("Added a wrong wall size");
            }
            _wallSize = wallSize;

            // Harcoding map: 
            for (int i = 0; i < sizeX; i++)
            {
                for (int j = 0; j < sizeZ; j++)
                {
                    if (j == 0  || j == sizeZ - 1)
                    {
                        _map[i, j] = WALL;
                    }
                    else if (i == 1 || i == 0)
                    {
                        _map[i, j] = '0';
                    }
                    else if (i == sizeX - 2 || i == sizeX - 1)
                    {
                        _map[i, j] = '1';
                    }
                    else
                    {
                        _map[i, j] = EMPTY;
                    }
                }
            }
        }

        /// <summary>
        /// Function that adds a wall to position posX posZ in the grid.
        /// </summary>
        /// <param name="posX">The horizontal position of center of the wall</param>
        /// <param name="posZ">The vertical position of center of the wall</param>
        /// <param name="isVertical">Boolean to set the orientation (vertical or horizontal) of the wall</param>
        /// /// <param name="errorMessage">Message for what failed in adding the wall</param>
        public bool AddWall(int posX, int posZ, bool isVertical, out string errorMessage)        {
            char[,] newMap;
            if (!CanAddWall(posX, posZ, isVertical, out newMap, out errorMessage)) return false;
            _map = newMap;
            return true;
        }

        public bool CanAddWall(int posX, int posZ, bool isVertical, out char[,] newMap, out string errorMessage)
        {
            newMap = (char[,]) _map.Clone();
            errorMessage = "";

            //WallSize is always even 
            for (int i = 0; i <= _wallSize/2; i++)
            {
                // Looks Bad   
                if (isVertical)
                {
                    if (!IsTilePossible(posX, posZ + i, newMap, out errorMessage))
                    {
                        return false;
                    }
                    newMap[posX, posZ + i] = WALL;
                    if (!IsTilePossible(posX, posZ - i, newMap, out errorMessage))
                    {
                        return false;
                    }
                    newMap[posX, posZ - i] = WALL;
                }
                else
                {
                    if (!IsTilePossible(posX + i, posZ, newMap, out errorMessage))
                    {
                        return false;
                    }
                    newMap[posX + i, posZ] = WALL;
                    if (!IsTilePossible(posX - i, posZ, newMap, out errorMessage))
                    {
                        return false;
                    }
                    newMap[posX - i, posZ] = WALL;
                }
            }

            foreach (char key in _players.Keys)
            {
                if (IsBlockingPlayer((char[,]) newMap.Clone(), key))
                {
                    errorMessage += "Wall Added is blocking player " + key + ".\n";
                    return false;
                }
            }
            return true;
        }

        private bool IsTilePossible(int posX, int posZ, char[,] map, out string errorMessage)
        {
            errorMessage = "";
            if (IsOutside(posX, posZ))
            {
                errorMessage = "Wall Added outside map.\n";
                return false;
            }
            if (_map[posX, posZ] == WALL)
            {
                errorMessage = "Wall Added on top of other.\n";
                return false;
            }
            if (char.IsDigit(_map[posX, posZ]))
            {
                errorMessage = "Wall Added on top of player: " + _map[posX, posZ] + ".\n";
                return false;
            }

            return true;
        }

        private bool IsOutside(int posX, int posZ)
        {
            return posX < 0 || posZ < 0 || posX >= _map.GetLength(0) || posZ >= _map.GetLength(1);
        }

        private bool IsBlockingPlayer(char[,] map, char player)
        {
            Func<Tuple<int, int>, int> bfs = null;
            Func<Tuple<int, int>, int, int, int> tryDoNext = (pos, x, z) =>
            {
                int r = 0;
                if (IsOutside(pos.Item1 + x, pos.Item2 + z)) return r;
                if (map[pos.Item1 + x, pos.Item2 + z] == player)
                {
                    r = 1;
                }
                if (map[pos.Item1 + x, pos.Item2 + z] == WALL || map[pos.Item1 + x, pos.Item2 + z] == RESERVED)
                    return r;

                map[pos.Item1 + x, pos.Item2 + z] = RESERVED;
                r += bfs(new Tuple<int, int>(pos.Item1 + x, pos.Item2 + z));
                return r;
            };
            bfs = (pos) =>
            {
                int ret = 0;
                ret += tryDoNext(pos, 1, 0);
                ret += tryDoNext(pos, 0, 1);
                ret += tryDoNext(pos, -1, 0);
                ret += tryDoNext(pos, 0, -1);
                return ret;
            };

            return bfs(_players[player]) < CountChar(player);
        }

        private int CountChar(char player)
        {
            int ret = 0;
            for (int i = 0; i < _map.GetLength(0); i++)
            {
                for (int j = 0; j < _map.GetLength(1); j++)
                {
                    if (_map[i, j] == player)
                    {
                        ret += 1;
                    }
                }
            }
            return ret;
        }

        public void UpdatePlayer(int posX, int posY, char player)
        {
            Tuple<int, int> pos;
            if (_players.TryGetValue(player, out pos))
            {
                _map[pos.Item1, pos.Item2] = EMPTY;
            }
            _players[player] = new Tuple<int, int>(posX, posY);
            _map[posX, posY] = player;
        }

        public override string ToString()
        {
            var s = "";
            for (int j = _map.GetLength(1) - 1; j >= 0; j--)
            {
                for (int i = 0; i < _map.GetLength(0); i++)
                {

                    s += _map[i, j];
                }
                s += '\n';
            }
            return s;
        }

        class Tuple<T1, T2>
        {
            public T1 Item1 { get; private set; }
            public T2 Item2 { get; private set; }

            public Tuple(T1 first, T2 second)
            {
                Item1 = first;
                Item2 = second;
            }
        }
    }
}