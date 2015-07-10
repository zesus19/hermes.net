using System;
using System.Text;

namespace Arch.CMessaging.Client.Core.Collections
{
	public class RedBlackNode
	{
        public static int RED = 0;
        public static int BLACK = 1;
		private IComparable ordKey;
		private object objData;
		private int intColor;
		private RedBlackNode rbnLeft;
		private RedBlackNode rbnRight;
        private RedBlackNode rbnParent;
		
		public IComparable Key
		{
			get
            {
				return ordKey;
			}
			
			set
			{
				ordKey = value;
			}
		}

		public object Data
		{
			get
            {
				return objData;
			}
			
			set
			{
				objData = value;
			}
		}

		public int Color
		{
			get
            {
				return intColor;
			}
			
			set
			{
				intColor = value;
			}
		}

		public RedBlackNode Left
		{
			get
            {
				return rbnLeft;
			}
			
			set
			{
				rbnLeft = value;
			}
		}

		public RedBlackNode Right
		{
			get
            {
				return rbnRight;
			}
			
			set
			{
				rbnRight = value;
			}
		}
        public RedBlackNode Parent
        {
            get
            {
                return rbnParent;
            }
			
            set
            {
                rbnParent = value;
            }
        }

		public RedBlackNode()
		{
			Color = RED;
		}
	}
}
