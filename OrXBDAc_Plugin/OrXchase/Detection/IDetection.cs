using System;
using System.Collections.Generic;

namespace OrXBDAc.chase
{
	interface IDetection
	{
		void UpdateMap (List<OrXchaseContainer> collection);
		void Debug();
	}
}

