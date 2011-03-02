using System;
using System.Collections;
using System.Collections.Generic;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace OpenTK_GL_Outline_example
{
	class MainWindow : GameWindow
	{
		//Define the cubes overall size at Class/Static level - Add 'offsets' to create individual cubes                
		//ftr
		protected static readonly Vector3 frontTopRightCorner = new Vector3 (1.0f, 1.0f, 0.0f);
		//ftl
		protected static readonly Vector3 frontTopLeftCorner = new Vector3 (0.0f, 1.0f, 0.0f);
		//fbl
		protected static readonly Vector3 frontBottomLeftCorner = new Vector3 (0.0f, 0.0f, 0.0f);
		//fbr
		protected static readonly Vector3 frontBottomRightCorner = new Vector3 (1.0f, 0.0f, 0.0f);

		//bbl
		protected static readonly Vector3 backBottomLeftCorner = new Vector3 (1.0f, 0.0f, -1.0f);
		//btl
		protected static readonly Vector3 backTopLeftCorner = new Vector3 (1.0f, 1.0f, -1.0f);
		//btr
		protected static readonly Vector3 backTopRightCorner = new Vector3 (0.0f, 1.0f, -1.0f);
		//bbr
		protected static readonly Vector3 backBottomRightCorner = new Vector3 (0.0f, 0.0f, -1.0f);


		public MainWindow () : base(800, 600)
		{
			//ftr - index 0
			vectorVertexList.Add(frontTopRightCorner);
			
			//ftl - index 1
			vectorVertexList.Add(frontTopLeftCorner);
			
			//fbl - index 2
			vectorVertexList.Add(frontBottomLeftCorner);
			
			//fbr - index 3
			vectorVertexList.Add(frontBottomRightCorner);
			
			//bbl - index 4
			vectorVertexList.Add(backBottomLeftCorner);
			
			//btl - index 5
			vectorVertexList.Add(backTopLeftCorner);
			
			//btr - index 6
			vectorVertexList.Add(backTopRightCorner);
			
			//bbr - index 7
			vectorVertexList.Add(backBottomRightCorner);
			
			
			//Actually extract the vector/index floats into a list (later becomes a float array).
			addVerticesToList(vectorVertexList);
			
			//Setup cube triangle indices (we could draw quads, this is some 'left over' code from Monotouch/iPhone, not bothered to change)
			//2 triangle per side
			indexList.Add (0);
			indexList.Add (1);
			indexList.Add (2);
			indexList.Add (2);
			indexList.Add (3);
			indexList.Add (0);
			
			//right side triangles
			indexList.Add (5);
			indexList.Add (0);
			indexList.Add (3);
			indexList.Add (3);
			indexList.Add (4);
			indexList.Add (5);
			
			//back side triangles
			indexList.Add (6);
			indexList.Add (5);
			indexList.Add (4);
			indexList.Add (4);
			indexList.Add (7);
			indexList.Add (6);
			
			//left side triangles
			indexList.Add (1);
			indexList.Add (6);
			indexList.Add (7);
			indexList.Add (7);
			indexList.Add (2);
			indexList.Add (1);
			
			//top side triangles
			indexList.Add (5);
			indexList.Add (6);
			indexList.Add (1);
			indexList.Add (1);
			indexList.Add (0);
			indexList.Add (5);
			
			//bottom side triangles
			indexList.Add (3);
			indexList.Add (2);
			indexList.Add (7);
			indexList.Add (7);
			indexList.Add (4);
			indexList.Add (3);
		}

		public uint vboVertexBuffer;
		public uint vboIndexBuffer;
		public uint vboColourBuffer;
		public List<float> vertexList = new List<float>();
		public List<ushort> indexList = new List<ushort>();
		public List<byte> colourList = new List<byte>();
		public List<Vector3> vectorVertexList = new List<Vector3>();

		private byte[] colorArray = { 
			255, 255, 255, 255, 
			255, 255, 255, 255 
		};


		protected override void OnLoad (EventArgs e)
		{
			base.OnLoad (e);

			MakeCurrent ();
			
			GL.GenBuffers (1, out vboVertexBuffer); //Create uint VBO buffer reference
			GL.BindBuffer (BufferTarget.ArrayBuffer, vboVertexBuffer);	//'activate' the selected VBO buffer, then set data (below)			
			GL.BufferData (BufferTarget.ArrayBuffer, new IntPtr ((sizeof(float) * vertexList.ToArray().Length)), vertexList.ToArray(), BufferUsageHint.StaticDraw);

			GL.GenBuffers (1, out vboIndexBuffer);	//Create uint indices buffer reference
			GL.BindBuffer (BufferTarget.ElementArrayBuffer, vboIndexBuffer);	//'activate' the selected indices buffer, then set data (below)
			GL.BufferData (BufferTarget.ElementArrayBuffer, new IntPtr ((sizeof(ushort) * indexList.ToArray().Length)), indexList.ToArray(), BufferUsageHint.StaticDraw);
			
			GL.GenBuffers (1, out vboColourBuffer);	//Create uint color buffer reference
			GL.BindBuffer (BufferTarget.ArrayBuffer, vboColourBuffer);	//'activate' the selected color buffer, then set data (below)
			GL.BufferData (BufferTarget.ArrayBuffer, new IntPtr ((sizeof(byte) * colourList.ToArray().Length)), colourList.ToArray(), BufferUsageHint.StaticDraw);
			
			GL.MatrixMode (MatrixMode.Modelview);
			GL.LoadIdentity ();
			
			//Is this equivalent to GLortho?
			Matrix4 perspective = Matrix4.CreatePerspectiveFieldOfView (MathHelper.DegreesToRadians (45f), ((float)Size.Width / (float)Size.Height), 0.1f, 10000f);
			unsafe {
				GL.LoadMatrix (&perspective.Row0.X);
			}			
			
			//GL.LoadMatrix (ref perspective);
			
			GL.Viewport (0, 0, Size.Width, Size.Height);
			
			//GL.Rotate (270f, 0f, 0f, 1f);
			GL.ShadeModel (ShadingModel.Flat);
			GL.ClearDepth (1f);
			GL.Enable (EnableCap.DepthTest);
			GL.DepthFunc (DepthFunction.Lequal);
			GL.Hint (HintTarget.PerspectiveCorrectionHint, HintMode.Nicest);
			
		}

		protected override void OnRenderFrame (FrameEventArgs e)
		{
			base.OnRenderFrame (e);
			
			MakeCurrent ();
			
			GL.ClearColor (0.5f, 0.5f, 0.7f, 1f);
			GL.Clear (ClearBufferMask.ColorBufferBit);
			GL.MatrixMode (MatrixMode.Modelview);
			
			Vector3 m_eye = new Vector3(0f, 0f, 5f);
			Vector3 m_target = new Vector3(0f, 0f, 0f);
			Vector3 m_up = new Vector3(0f, 1f, 0f);
			
			//Matrix4 matrix = Matrix4.LookAt (50f, 100f, 100f, 0f, 0f, 0f, 0f, 0f, 1f);
			Matrix4 matrix = Matrix4.LookAt (m_eye, m_target, m_up);
			unsafe {
				GL.LoadMatrix (&matrix.Row0.X);
			}
			//GL.LoadMatrix(ref matrix.Row0.X);
			
			
			//Handle Vertex VBO data
			GL.BindBuffer (BufferTarget.ArrayBuffer, vboVertexBuffer);
			GL.EnableClientState (ArrayCap.VertexArray);
			//3 values per vertex, as normal (xyz)
			GL.VertexPointer (3, VertexPointerType.Float, 0, IntPtr.Zero);
			
			//Use the indices to draw the required indices
			GL.BindBuffer (BufferTarget.ElementArrayBuffer, vboIndexBuffer);
			GL.DrawElements (BeginMode.Triangles, indexList.ToArray().Length, DrawElementsType.UnsignedShort, IntPtr.Zero);	
			
			//Handle Color VBO data
			GL.BindBuffer (BufferTarget.ArrayBuffer, vboColourBuffer);
			GL.EnableClientState (ArrayCap.ColorArray);
			//Color uses groups of 4 values per index here (rgba)
			GL.ColorPointer (4, ColorPointerType.UnsignedByte, 0, IntPtr.Zero);
		
			//'Switch off' Client render states
			GL.DisableClientState (ArrayCap.VertexArray);
			GL.DisableClientState (ArrayCap.ColorArray);
			
			SwapBuffers();
		}
		
		protected void addVerticesToList (List<Vector3> _candidateVertexList)
		{
			vertexList.Clear();
			
			IEnumerator<Vector3> vec3Enum = _candidateVertexList.GetEnumerator();
			
			while(vec3Enum.MoveNext()) {
				Vector3 nextVertex = vec3Enum.Current;				
				
				vertexList.Add (nextVertex.X);
				vertexList.Add (nextVertex.Y);
				vertexList.Add (nextVertex.Z);
			}
		}
		
		public override void Dispose() 
		{
			GL.DeleteBuffers (1, ref vboVertexBuffer);
			GL.DeleteBuffers (1, ref vboIndexBuffer);
			GL.DeleteBuffers (1, ref vboColourBuffer);
			
			base.Dispose();
		}
		
	}

	class MainClass
	{
		/// <summary>
		/// Entry point of this example.
		/// </summary>
		[STAThread]
		public static void Main (string[] args)
		{
			using (MainWindow example = new MainWindow ()) {
				example.Title = "OpenGL outline offset example.";
				example.Run (30.0, 0.0);
			}
		}
	}
}

