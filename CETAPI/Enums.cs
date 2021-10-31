using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CETAPI.Enums
{
    /// <summary>
    /// Enum describing possible side states
    /// </summary>
    public enum Side
    {
        Ask = 1,
        Bid,
    }

    /// <summary>
    /// Enum describing possible action types
    /// </summary>
    public enum ActionType
    {
        Insert = 1,
        Update,
        Delete,
    }
}
