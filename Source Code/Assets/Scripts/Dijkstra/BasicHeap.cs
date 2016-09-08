using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

    class BasicHeap 
    {
        private List<HeapNode> InnerList = new List<HeapNode>();

        public BasicHeap() {}

        public int Count
        {
            get
            {
                return InnerList.Count;
            }
        }



	public float getWight(int index){
		HeapNode result = InnerList[index];
		return result.weight;
	}

	public float getIndex(int index){
		HeapNode result = InnerList[index];
		return result.index;
	}





        public void Push(int index, float weight)
        {
            int p = InnerList.Count, p2;
            InnerList.Add(new HeapNode(index,weight)); 
            do
            {
                if (p == 0)
                    break;

                p2 = (p - 1) >> 1;
                if (InnerList[p].weight<InnerList[p2].weight)
                {
                    HeapNode h = InnerList[p];
                    InnerList[p] = InnerList[p2];
                    InnerList[p2] = h;

                    p = p2;
                }
                else
                    break;
            } while (true);
        }

        public int Pop()
        {
            HeapNode result = InnerList[0];
            int p = 0, p1, p2, pn;
            InnerList[0] = InnerList[InnerList.Count - 1];
            InnerList.RemoveAt(InnerList.Count - 1);
            do
            {
                pn = p;
                p1 = (p << 1) + 1;
                p2 = (p << 1)  + 2;
                if (InnerList.Count > p1 && InnerList[p].weight > InnerList[p1].weight) 
                    p = p1;
                if (InnerList.Count > p2 && InnerList[p].weight > InnerList[p2].weight) 
                    p = p2;

                if (p == pn)
                    break;
                
                HeapNode h = InnerList[p];
                InnerList[p] = InnerList[pn];
                InnerList[pn] = h;

            } while (true);
            return result.index;
        }
    }
