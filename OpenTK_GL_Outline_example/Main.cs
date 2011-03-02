using System;
using System.Drawing;
using System.Collections;
using System.Collections.Generic;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;
using System.Runtime.InteropServices;

namespace OpenTK_GL_Outline_example
{

	/*
		From http://www.google.com/codesearch/p?hl=en#q2SoGSWrDvE/trunk/src/MainForm.cs&q=lang:c%23%20%22GL.ClearDepth%22%20opentk&sa=N&cd=2&ct=rc
		GL.PolygonOffset(1.0f, -1.0f);
		GL.EnableClientState(EnableCap.PolygonOffsetFill);

		See also https://github.com/benjcooley/sable-fx -> http://www.google.com/codesearch/p?hl=en#fR0ejzflOaE/ThirdParty/OpenTK-1.0/Source/Examples/OpenGL/1.1/StencilCSG.cs&q=lang:c%23%20%22GL.ClearDepth%22%20opentk&sa=N&cd=3&ct=rc
	*/
	class MainWindow : GameWindow
	{
		//Variables-----\
        private bool fullscreen;
        //Variables-----/
		
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


		public MainWindow () : base(800, 600, GraphicsMode.Default, "OpenGL outline offset example.")
		{
            VSync = VSyncMode.On;
			
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

			colourList.AddRange(colorArray);
			
			//right side triangles
			indexList.Add (5);
			indexList.Add (0);
			indexList.Add (3);
			indexList.Add (3);
			indexList.Add (4);
			indexList.Add (5);

			colourList.AddRange(colorArray);
			
			//back side triangles
			indexList.Add (6);
			indexList.Add (5);
			indexList.Add (4);
			indexList.Add (4);
			indexList.Add (7);
			indexList.Add (6);

			colourList.AddRange(colorArray);
			
			//left side triangles
			indexList.Add (1);
			indexList.Add (6);
			indexList.Add (7);
			indexList.Add (7);
			indexList.Add (2);
			indexList.Add (1);

			colourList.AddRange(colorArray);
			
			//top side triangles
			indexList.Add (5);
			indexList.Add (6);
			indexList.Add (1);
			indexList.Add (1);
			indexList.Add (0);
			indexList.Add (5);

			colourList.AddRange(colorArray);
			
			//bottom side triangles
			indexList.Add (3);
			indexList.Add (2);
			indexList.Add (7);
			indexList.Add (7);
			indexList.Add (4);
			indexList.Add (3);

			colourList.AddRange(colorArray);
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
			
			GL.MatrixMode (MatrixMode.Projection);
			GL.LoadIdentity ();
			
			//Is this equivalent to GLortho?
			Matrix4 perspective = Matrix4.CreatePerspectiveFieldOfView (MathHelper.DegreesToRadians (45f), ((float)Size.Width / (float)Size.Height), 0.1f, 10000f);
			/*
			unsafe {
				GL.LoadMatrix (&perspective.Row0.X);
			}
			*/	
			
			GL.LoadMatrix (ref perspective);
			
			GL.Viewport(ClientRectangle.X, ClientRectangle.Y, ClientRectangle.Width, ClientRectangle.Height);
			//GL.Viewport (0, 0, Size.Width, Size.Height);
			
			//GL.Rotate (270f, 0f, 0f, 1f);
			GL.ShadeModel (ShadingModel.Flat);
			GL.Enable (EnableCap.DepthTest);
			GL.DepthFunc (DepthFunction.Lequal);

			// Clear the back buffer.
			GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
			//GL.ClearDepth (1f);

			GL.Hint (HintTarget.PerspectiveCorrectionHint, HintMode.Nicest);
			
			//Initialise GL VBO Buffers
			GL.GenBuffers (1, out vboVertexBuffer); //Create uint VBO buffer reference
			GL.BindBuffer (BufferTarget.ArrayBuffer, vboVertexBuffer);	//'activate' the selected VBO buffer, then set data (below)			
			GL.BufferData (BufferTarget.ArrayBuffer, new IntPtr ((sizeof(float) * vertexList.ToArray().Length)), vertexList.ToArray(), BufferUsageHint.StaticDraw);

			GL.GenBuffers (1, out vboIndexBuffer);	//Create uint indices buffer reference
			GL.BindBuffer (BufferTarget.ElementArrayBuffer, vboIndexBuffer);	//'activate' the selected indices buffer, then set data (below)
			GL.BufferData (BufferTarget.ElementArrayBuffer, new IntPtr ((sizeof(ushort) * indexList.ToArray().Length)), indexList.ToArray(), BufferUsageHint.StaticDraw);
			
			GL.GenBuffers (1, out vboColourBuffer);	//Create uint color buffer reference
			GL.BindBuffer (BufferTarget.ArrayBuffer, vboColourBuffer);	//'activate' the selected color buffer, then set data (below)
			GL.BufferData (BufferTarget.ArrayBuffer, new IntPtr ((sizeof(byte) * colourList.ToArray().Length)), colourList.ToArray(), BufferUsageHint.StaticDraw);
			
		}

        protected override void OnResize(EventArgs e) {
            base.OnResize(e);

            GL.Viewport(ClientRectangle.X, ClientRectangle.Y, ClientRectangle.Width, ClientRectangle.Height);

            GL.MatrixMode(MatrixMode.Projection);
            Matrix4 projection = Matrix4.CreatePerspectiveFieldOfView((float)Math.PI / 4, Width / (float)Height, .1f, 1000f);
			//Matrix4 projection = Matrix4.CreatePerspectiveFieldOfView (MathHelper.DegreesToRadians (45f), ((float)Size.Width / (float)Size.Height), 0.1f, 10000f);
            GL.LoadMatrix(ref projection);

            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadIdentity();
        }

        protected override void OnUpdateFrame(FrameEventArgs e) {
            if (Keyboard[Key.Escape])
                Exit();

            if (Keyboard[Key.F1]) {
                fullscreen = !fullscreen;
                if (fullscreen)
                    WindowState = WindowState.Fullscreen;
                else
                    WindowState = WindowState.Normal;
            }
        }

		protected override void OnRenderFrame (FrameEventArgs e)
		{
			base.OnRenderFrame (e);
			
			MakeCurrent ();
			
			GL.ClearColor (0.5f, 0.5f, 0.7f, 1f);
			GL.Clear (ClearBufferMask.ColorBufferBit);
			GL.MatrixMode (MatrixMode.Modelview);
			
			Vector3 m_eye = new Vector3(-3f, 3f, 5f);
			Vector3 m_target = new Vector3(0.5f, 0.5f, 0.5f);
			Vector3 m_up = new Vector3(0f, 1f, 0f);
			
			//Matrix4 matrix = Matrix4.LookAt (50f, 100f, 100f, 0f, 0f, 0f, 0f, 0f, 1f);
			Matrix4 matrix = Matrix4.LookAt (m_eye, m_target, m_up);
			/*
			unsafe {
				GL.LoadMatrix (&matrix.Row0.X);
			}
			*/
			GL.LoadMatrix(ref matrix.Row0.X);
			
			
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
			
			updateFPS();
			
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

        long time = DateTime.Now.Ticks;
        private void updateFPS() {
            if (DateTime.Now.Ticks - time > 10000000) {
#if (DEBUG)
                Console.WriteLine(time);
#endif
                time = DateTime.Now.Ticks;
                Title = "FPS: " + RenderFrequency;
            }
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
				//example.Title = "OpenGL outline offset example.";
				example.Run (30.0, 0.0);
			}
		}
	}
}

