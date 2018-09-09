using System;
using System.Collections.Generic;
using System.Text;

namespace OrXBDAc.chase
{
    /// <summary>
    /// The mode the EVA is in.
    /// </summary>
    /// 
    [Flags]
    public enum Mode
    {
        None = 1,
        Follow = 2,
        Patrol = 3,
        Leader = 4,
        Order = 5,
		Wander = 6
    }

    /// <summary>
    /// The status the EVA is in.
    /// </summary>
    /// 
    [Flags]
    public enum Status
    {
        None,
        Removed
    }

    /// <summary>
    /// The animation states for the OrXchaseControllerContainer
    /// </summary>
    enum AnimationState
    {
        None,
        Swim,
        Run,
        Walk,
        BoundSpeed,
        Idle
    }
}
