

using System;

public struct GridPosition : IEquatable<GridPosition>
{
    #region Variables
    public int x;
    public int z;
    #endregion


    #region ctor
    public GridPosition(int x, int z)
    {
        this.x = x;
        this.z = z;
    }
    
    public GridPosition(string position)
    {
        if (string.IsNullOrEmpty(position))
        {
            throw new ArgumentException("Position string cannot be null or empty.");
        }

        // Remove the braces
        position = position.Trim('{', '}');

        // Split on comma
        var parts = position.Split(',');

        if (parts.Length != 2)
        {
            throw new ArgumentException("Position string should be in the format '{x:n,z:m}'.");
        }

        int? tempX = null;
        int? tempZ = null;

        foreach (var part in parts)
        {
            var keyValue = part.Split(':');
            if (keyValue.Length != 2)
            {
                throw new ArgumentException("Invalid format for position.");
            }

            var key = keyValue[0].Trim();
            if (!int.TryParse(keyValue[1].Trim(), out var value))
            {
                throw new ArgumentException($"Invalid value for key {key}.");
            }

            if (key == "x")
            {
                tempX = value;
            }
            else if (key == "z")
            {
                tempZ = value;
            }
            else
            {
                throw new ArgumentException($"Unexpected key {key} in position.");
            }
        }

        if (!tempX.HasValue || !tempZ.HasValue)
        {
            throw new ArgumentException("Position string should contain both x and z values.");
        }

        x = tempX.Value;
        z = tempZ.Value;
    }
    #endregion

    #region Overrides
    public override bool Equals(object obj)
    {
        return obj is GridPosition position &&
               x == position.x &&
               z == position.z;
    }

    public bool Equals(GridPosition other)
    {
        return this == other;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(x, z);
    }

    public override string ToString()
    {
        return "\"{x:" + x + ",z:" + z + "}\"";
    }

    public static bool operator== (GridPosition left, GridPosition right)
    {
        return left.x == right.x && left.z == right.z;
    }

    public static bool operator!= (GridPosition left, GridPosition right)
    {
        return !(left == right);
    }

    public static GridPosition operator+ (GridPosition left, GridPosition right)
    {
        return new GridPosition(left.x + right.x, left.z + right.z);
    }

    public static GridPosition operator -(GridPosition left, GridPosition right)
    {
        return new GridPosition(left.x - right.x, left.z - right.z);
    }

    #endregion

}