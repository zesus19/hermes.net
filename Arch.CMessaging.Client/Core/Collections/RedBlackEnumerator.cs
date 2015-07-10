using System;
using System.Collections;

namespace Arch.CMessaging.Client.Core.Collections
{
	public class RedBlackEnumerator
	{
		private Stack stack;
		private bool keys;
		private bool ascending;
		
		private IComparable ordKey;
		private object objValue;
        private RedBlack redBlack;

        public  string  Color;             
        public  IComparable parentKey;     
		

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

		public object Value
		{
			get
            {
				return objValue;
			}
			
			set
			{
				objValue = value;
			}
		}

		public RedBlackEnumerator(RedBlack redBlack, RedBlackNode tnode, bool keys, bool ascending) 
        {

            this.stack = new Stack();
            this.keys = keys;
            this.ascending = ascending;
            this.redBlack = redBlack;
			
            if(ascending)
			{  
                while (tnode != redBlack.sentinelNode)
				{
					stack.Push(tnode);
					tnode = tnode.Left;
				}
			}
			else
			{
                while (tnode != redBlack.sentinelNode)
				{
					stack.Push(tnode);
					tnode = tnode.Right;
				}
			}
			
		}

		public bool HasMoreElements()
		{
			return (stack.Count > 0);
		}

		public object NextElement()
		{
			if(stack.Count == 0)
				throw(new RedBlackException("Element not found"));
			
			RedBlackNode node = (RedBlackNode) stack.Peek();	
			
            if(ascending)
            {
                if (node.Right == redBlack.sentinelNode)
                {	
                    RedBlackNode tn = (RedBlackNode) stack.Pop();
                    while(HasMoreElements()&& ((RedBlackNode) stack.Peek()).Right == tn)
                        tn = (RedBlackNode) stack.Pop();
                }
                else
                {
                    RedBlackNode tn = node.Right;
                    while (tn != redBlack.sentinelNode)
                    {
                        stack.Push(tn);
                        tn = tn.Left;
                    }
                }
            }
            else            
            {
                if (node.Left == redBlack.sentinelNode)
                {
                    RedBlackNode tn = (RedBlackNode) stack.Pop();
                    while(HasMoreElements() && ((RedBlackNode)stack.Peek()).Left == tn)
                        tn = (RedBlackNode) stack.Pop();
                }
                else
                {
                    RedBlackNode tn = node.Left;
                    while (tn != redBlack.sentinelNode)
                    {
                        stack.Push(tn);
                        tn = tn.Right;
                    }
                }
            }
			
            Key = node.Key;
            Value = node.Data;
            try
            {
                parentKey = node.Parent.Key;            
            }
            catch(Exception e)
            {
                object o = e;                       
                parentKey = 0;
            }
			if(node.Color == 0)                    
                Color = "Red";
            else
                Color = "Black";

            return keys == true ? node.Key : node.Data;			
		}

		public bool MoveNext()
		{
			if(HasMoreElements())
			{
				NextElement();
				return true;
			}
			return false;
		}
	}
}
