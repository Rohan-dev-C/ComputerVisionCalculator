using Emgu.CV;

using System;
using System.Collections.Generic;
using System.Text;

namespace Emgu.CV
{
    public class HierarchyMatrix : IDisposable, IOutputArray
    {
        public readonly Mat Matrix = new Mat();

        /// <param name="component">next, previous, child, parent</param>
        /// <param name="contour">contour to read from</param>
        public int this[int contour, int component]
            => Get(contour, component);

        /// <param name="contour">contour to read from</param>
        public HierarchyItem this[int contour]
            => new HierarchyItem(Get(contour, 0), Get(contour, 1), Get(contour, 2), Get(contour, 3));

        /// <param name="component">next, previous, child, parent</param>
        /// <param name="contourIndex">contour to read from</param>
        int Get(int contourIndex, int component)
        {
            //element stride is the amount of ints wide each element is
            long elementStride = Matrix.ElementSize / sizeof(Int32);
            //offset is the position within the matrix 
            //(think a 2D array of a type with a size of elementStride.
            var offset = component + contourIndex * elementStride;

            //if the index is within bounds
            if (offset >= 0 && offset < Matrix.Total.ToInt64() * elementStride)
            {
                unsafe
                {
                    //get the value that is at the matrix array pointer + offset
                    return *((int*)Matrix.DataPointer.ToPointer() + offset);
                }
            }
            return -1;
        }

        InputArray IInputArray.GetInputArray() => ((IInputArray)Matrix).GetInputArray();
        OutputArray IOutputArray.GetOutputArray() => ((IOutputArray)Matrix).GetOutputArray();

        public void Dispose() => Matrix.Dispose();
    }

    public readonly struct HierarchyItem
    {
        /// <summary>
        /// The index of the next contour at this hierarchy level. 
        /// -1 if unavailable.
        /// </summary>
        public readonly int Next;
        /// <summary>
        /// The index of the previous contour at this hierarchy level. 
        /// -1 if unavailable.
        /// </summary>
        public readonly int Previous;
        /// <summary>
        /// The index of the first child contour at this hierarchy level. 
        /// -1 if unavailable.
        /// </summary>
        public readonly int FirstChild;
        /// <summary>
        /// The index of the parent contour at this hierarchy level. 
        /// -1 if unavailable.
        /// </summary>
        public readonly int Parent;

        public HierarchyItem(int next, int previous, int firstChild, int parent)
        {
            Next = next;
            Previous = previous;
            FirstChild = firstChild;
            Parent = parent;
        }
    }
}