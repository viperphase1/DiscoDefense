/* C# port of QHull. See 'QHull Licence.txt' for license details */

namespace Technie.PhysicsCreator.QHull
{

	/**
	 * Maintains a single-linked list of faces
	 */
	public class FaceList
	{
		private Face head;
		private Face tail;

		/**
		 * Clears this list.
		 */
		public void clear()
		{
			head = tail = null;
		}

		/**
		 * Adds a vertex to the end of this list.
		 */
		public void add(Face vtx)
		{
			if (head == null)
			{
				head = vtx;
			}
			else
			{
				tail.next = vtx;
			}
			vtx.next = null;
			tail = vtx;
		}

		public Face first()
		{
			return head;
		}

		/**
		 * Returns true if this list is empty.
		 */
		public bool isEmpty()
		{
			return head == null;
		}
	}

} // namespace Technie.PhysicsCreator.QHull
