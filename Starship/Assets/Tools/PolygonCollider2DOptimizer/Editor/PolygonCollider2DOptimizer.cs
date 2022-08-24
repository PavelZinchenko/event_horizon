// PolygonCollider2D Optimizer by Unitycoder.com

using UnityEditor;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace unitycodercom_PolygonCollider2DOptimizer
{

	public class PolygonCollider2DOptimizer : EditorWindow 
	{

		private const string appName = "PolygonCollider2D Optimizer";
		private float angleThreshold = 18.0f; // 18 seems good values
		private float shrinkMultiplier = 0.98f; // 0.98 seems good value
		private int filterAmount = 4;
		private float moveStep = 0.01f;
		private string[] scalingModes = new string[]{"Estimate Neighbors","Standard Scaling","Normalized Scaling", "Normalized Scaling (local center)"};
		private int scalingMode = 3;
		private int origVertCount = 0;
		private int cleanVertCount = 0;
		private int removedVertPercent = 0;
		private string selectionString="\n\n";

		// create menu item and window
		[MenuItem ("Window/PolygonColliderOptimizer/"+appName, false, 1)]
		static void Init () 
		{
			PolygonCollider2DOptimizer window = (PolygonCollider2DOptimizer)EditorWindow.GetWindow (typeof (PolygonCollider2DOptimizer));
			window.titleContent= new GUIContent(appName);
			window.minSize = new Vector2(340,544);
			window.maxSize = new Vector2(340,548);

		}


		// update stats on change
		void OnSelectionChange()
		{
			CalculateSelection();
		}


		// main loop
		void OnGUI () 
		{
			// title
			GUILayout.Label ("SELECTION", EditorStyles.boldLabel);

			// info
			GUILayout.Label(selectionString);
//			GUILayout.Label("Selected objects:" + Selection.gameObjects.Length);
			EditorGUILayout.Space();

			// OPTIMIZE
			GUILayout.Label ("OPTIMIZE", EditorStyles.boldLabel);
			//angleThreshold = Mathf.Clamp(EditorGUILayout.FloatField(new GUIContent("Angle Threshold (0-360)",null,""), angleThreshold),0,360);
			GUILayout.BeginHorizontal();
			GUILayout.Label("Angle Threshold: "+angleThreshold, GUILayout.Width(150));
			angleThreshold = (int)GUILayout.HorizontalSlider(angleThreshold, 0, 90);
			GUILayout.EndHorizontal();
			//GUILayout.Label ("Removes connected vertices below this angle", EditorStyles.miniBoldLabel);
			if(GUILayout.Button (new GUIContent ("Optimize", ""),GUILayout.Height(30))) 
			{
				OptimizeColliders();
			}
			GUILayout.BeginHorizontal();
			if(GUILayout.Button (new GUIContent ("Remove inner paths", "Removes internal collider paths"), GUILayout.Height(20))) 
			{
				RemoveInternalPathsComplex();
			}
			if(GUILayout.Button (new GUIContent ("Keep largest path only", ""), GUILayout.Height(20))) 
			{
				KeepLargestPath();
			}
			GUILayout.EndHorizontal();

			GUILayout.BeginHorizontal();
			if(GUILayout.Button (new GUIContent ("Remove paths with less than", ""), GUILayout.Height(20))) 
			{
				FilterPaths(filterAmount);
			}
			filterAmount = (int)Mathf.Clamp(EditorGUILayout.IntField(new GUIContent("",".."), filterAmount, GUILayout.Width(30), GUILayout.Height(20)),2,4096);
			GUILayout.Label ("Vertices",EditorStyles.label,GUILayout.Height(20));
			GUILayout.EndHorizontal();

			/*
			GUILayout.BeginHorizontal();
			if(GUILayout.Button (new GUIContent ("Remove Path #", ""), GUILayout.Height(20)))
			{
				//RemovePathNumber();
			}
			pathToRemove = EditorGUILayout.IntField( new GUIContent("",".."), pathToRemove, GUILayout.Width(30), GUILayout.Height(20) );
			GUILayout.EndHorizontal();
			*/


			EditorGUILayout.Space();

			// SCALING
			GUILayout.Label ("SCALE", EditorStyles.boldLabel);
			GUILayout.BeginHorizontal();
			GUILayout.Label (new GUIContent("Scaling method",""));
			scalingMode = EditorGUILayout.Popup(scalingMode,scalingModes);
			GUILayout.EndHorizontal();
			shrinkMultiplier = EditorGUILayout.FloatField(new GUIContent("Scaling multiplier",null,".."), shrinkMultiplier);
			GUILayout.BeginHorizontal();
			if(GUILayout.Button (new GUIContent ("< Scale"),GUILayout.Height(30))) 
			{
				ScaleColliders(true);
			}
			if(GUILayout.Button (new GUIContent ("Scale >"),GUILayout.Height(30))) 
			{
				ScaleColliders(false);
			}
			GUILayout.EndHorizontal();
			EditorGUILayout.Space();

			/*
//			GUI.enabled = sourceFile==null?false:true; // disabled if no source selected
			//GUI.enabled = true;
			*/

			// MOVE
			GUILayout.Label ("MOVE", EditorStyles.boldLabel);
			moveStep = EditorGUILayout.FloatField(new GUIContent("Move step",null,".."), moveStep);
			GUILayout.BeginHorizontal();
			if(GUILayout.Button (new GUIContent ("<"),GUILayout.Height(30))) 
			{
				PanColliders(new Vector2(-moveStep,0));
			}
			if(GUILayout.Button (new GUIContent ("/\\"),GUILayout.Height(30))) 
			{
					PanColliders(new Vector2(0,moveStep));
					             }
			if(GUILayout.Button (new GUIContent ("\\/"),GUILayout.Height(30))) 
			{
						PanColliders(new Vector2(0,-moveStep));
             }
			if(GUILayout.Button (new GUIContent (">"),GUILayout.Height(30))) 
			{
				PanColliders(new Vector2(moveStep,0));
			}
			GUILayout.EndHorizontal();


			// OPTIONS
			EditorGUILayout.Space();
			EditorGUILayout.Space();
			GUILayout.Label ("OPTIONS", EditorStyles.boldLabel);
			GUILayout.BeginHorizontal();
			if(GUILayout.Button (new GUIContent ("Add Polygon2D colliders", "Adds PolygonCollider2D, if there are no 2D colliders"), GUILayout.Height(20))) 
			{
				AddPolygon2DCollider();
			}
			// REMOVE
			if(GUILayout.Button (new GUIContent ("Remove Polygon2D colliders", "Remove Polygon2D colliders"), GUILayout.Height(20))) 
			{
				RemovePolygon2DCollider();
			}
			GUILayout.EndHorizontal();


			// TODO: remove pol2d colliders
			// TODO: remove all points/paths?

			// RESET
			if(GUILayout.Button (new GUIContent ("Reset Polygon2D colliders", "Reset Polygon2D colliders (keeps trigger & material settings)"), GUILayout.Height(30))) 
			{
				ResetPolygon2DCollider();
			}


			// STATS
			EditorGUILayout.Space();
			GUILayout.Label ("STATISTICS", EditorStyles.boldLabel);
			GUILayout.Label ("Vertices : "+origVertCount+" > "+cleanVertCount + " ("+removedVertPercent+"% removed)");
		} // ongui



		// optimizes selected sprites
		void OptimizeColliders()
		{
			// reset vars
			origVertCount = 0;
			cleanVertCount = 0;
			removedVertPercent = 0;

			// loop all selected objects
			foreach (GameObject go in Selection.gameObjects)
			{
				// validate selected object
				if (go.GetComponent<SpriteRenderer>()!=null)
				{
					if (go.GetComponent<PolygonCollider2D>()==null)
					{
						/*
						// if need to add colliders
						if (go.GetComponent<BoxCollider2D>()==null &&
						    go.GetComponent<CircleCollider2D>()==null &&
						    go.GetComponent<EdgeCollider2D>()==null)
						{
							go.AddComponent<PolygonCollider2D>();
						}else{ // missing collider and not adding
							//Debug.LogWarning(appName+"> \""+go.name+"\" is missing PolygonCollider2D - skipping!");
						}*/
						continue; // no polygon2D collider
					}
				}else{ // skip non sprite objects
					continue;
				}

				PolygonCollider2D pc = go.GetComponent<PolygonCollider2D>();
				List<Vector2> newVerts = new List<Vector2>();

				for (int i=0;i<pc.pathCount;i++)
				{
					newVerts.Clear();
					// get collider
					Vector2[] path = pc.GetPath(i);

					if (path.Length<4)
					{
						// skipped, not enough vertices
						continue;
					}

					// TODO: check if has more than 0

					// get count for stats
					origVertCount+=path.Length;

//					int counter=0;

					float angle1=0;
					float angle2=0;
//					float angle3=0;

					// get first angle
					//counter++;

					// reset lists
					newVerts.Clear();
					// loop all points
					//while (counter<path.Length)
					int mx = path.Length;

					//angle1 = Vector2.Angle(path[mx-1]-path[0],Vector2.up);
					Vector2 currentDir = (path[0]-path[1]).normalized;
					angle1 = Vector2.Angle(path[0]-path[1],currentDir);


//					int sCurrent = 0;

					for (int j=0;j<mx;j++)
					{
						int sPrev = (((j-1) % mx) + mx) % mx;
						int sNext = (((j+1) % mx) + mx) % mx;

//						angle1 = Vector2.Angle(path[sPrev]-path[j],Vector2.up);
						angle1 = Vector2.Angle(path[sPrev]-path[j],currentDir);
						angle2 = Vector2.Angle(path[j]-path[sNext],currentDir);
//						angle2 = Vector2.Angle(path[sCurrent]-path[j],Vector2.up);
						//angle3 = Vector2.Angle(path[sPrev]-path[sNext],Vector2.up);

						//Debug.Log ("sPrev:" +sPrev+" current:"+j+" sNext:"+sNext+ " a1:"+angle1+" a2:"+angle2+" | angleDif:"+Mathf.Abs(angle2-angle1));

						// if bigger than threshold, add point
//						if (Mathf.Abs(angle2-angle1)>angleThreshold)
						if (angle1>angleThreshold || angle2>angleThreshold)
						{
						//	angle1 = Vector2.Angle(path[j]-path[sNext],Vector2.up);
//							angle1 = Vector2.Angle(path[sPrev]-path[j],Vector2.up);
							//sCurrent = j;
							currentDir = (path[j]-path[sNext]).normalized;

							newVerts.Add(path[j]);
						}
						
					//	counter++;
						
					} // looped each vert

					// update collider
					pc.SetPath(i,newVerts.ToArray());
					cleanVertCount+=newVerts.Count;

				} // each path
			} // for each gameobject

			// calculate statistics
			if (cleanVertCount>0)
			{
				removedVertPercent = 100-(int)((float)cleanVertCount/(float)origVertCount*100.0f);
			}else{
				removedVertPercent = 0;
			}

			CalculateSelection();

		} // OptimizeColliders




		// scaling
		void ScaleColliders(bool scaleDown)
		{
			// loop all selected objects
			foreach (GameObject go in Selection.gameObjects)
			{
				// validate selected object
				if (go.GetComponent<SpriteRenderer>()!=null)
				{
					if (go.GetComponent<PolygonCollider2D>()==null)
					{
						continue; // no pol2d collider
					}
				}else{ // skip non sprite objects
					continue;
				}
				

				PolygonCollider2D pc = go.GetComponent<PolygonCollider2D>();
				List<Vector2> newVerts = new List<Vector2>();

				// loop all paths
				for (int i=0;i<pc.pathCount;i++)
				{
					Vector2[] path = pc.GetPath(i);

					// reset lists
					newVerts.Clear();

					// shrink
					int mx = path.Length;
					for (int s=0;s<mx;s++)
					{
						switch (scalingMode)
						{
						case 0: // neighbour
							int sPrev = (((s-1) % mx) + mx) % mx;
							int sNext = (((s+1) % mx) + mx) % mx;
							Vector2 pdir = (path[sPrev]-path[sNext]).normalized;
							if (scaleDown)
							{
								newVerts.Add(path[s]-GetPerpendicular(pdir)*(1-shrinkMultiplier));
							}else{
								newVerts.Add(path[s]+GetPerpendicular(pdir)*(1-shrinkMultiplier));
							}
							break;
						case 1: // default
							if (scaleDown)
							{
								newVerts.Add(path[s]*shrinkMultiplier);
							}else{
								newVerts.Add(path[s]*(1+(1-shrinkMultiplier)));
							}
							break;
						case 2: // normalized
							if (scaleDown)
							{
								newVerts.Add(path[s]-path[s].normalized*(1-shrinkMultiplier));
							}else{
								newVerts.Add(path[s]+path[s].normalized*(1-shrinkMultiplier));
							}
							break;
						case 3: // normalized Centroid
							Vector2 center = GetCentroid(path);
							if (scaleDown)
							{
								Vector2 temp = (path[s]-center)-(path[s]-center).normalized*(1-shrinkMultiplier);
								newVerts.Add( temp+center );
							}else{
								Vector2 temp = path[s]-center+(path[s]-center).normalized*(1-shrinkMultiplier);
								newVerts.Add(temp+center);
							}
							break;
						default:
							Debug.LogWarning(appName+"> " + "Unknown scaling mode:"+scalingMode);
							break;
						}

						// TODO: scale based on local path center?
						
						
					} // looped each vert
					// update collider
					pc.SetPath(i,newVerts.ToArray());
				} // each path

				// cleanup
				newVerts.Clear();
			} // for each gameobject
		} // ScaleColliders()



		// move all verts
		void PanColliders(Vector2 moveDir)
		{
			// loop all selected objects
			foreach (GameObject go in Selection.gameObjects)
			{
				// validate selected object
				if (go.GetComponent<SpriteRenderer>()!=null)
				{
					if (go.GetComponent<PolygonCollider2D>()==null)
					{
						continue;
					}
				}else{ // skip non sprite objects
					continue;
				}

				PolygonCollider2D pc = go.GetComponent<PolygonCollider2D>();
				List<Vector2> newVerts = new List<Vector2>();

				// TODO: multiple paths
				for (int i=0;i<pc.pathCount;i++)
				{
					Vector2[] path = pc.GetPath(i);
					// reset lists
					newVerts.Clear();
					newVerts.InsertRange(0,path);
					// shrink
					int mx = newVerts.Count;
					for (int s=0;s<mx;s++)
					{
						newVerts[s]+=moveDir;
					}
					// update collider
					pc.SetPath(i,newVerts.ToArray());
				}
			} // for each gameobject
		} // PanColliders()



		// resets collider on selected object(s)
		void ResetPolygon2DCollider()
		{
			// reset stats
			origVertCount = 0;
			cleanVertCount = 0;
			removedVertPercent = 0;

			foreach (GameObject go in Selection.gameObjects)
			{
				if (go.GetComponent<PolygonCollider2D>()!=null)
				{
					bool isTrigger = go.GetComponent<PolygonCollider2D>().isTrigger;
					PhysicsMaterial2D physMat = go.GetComponent<PolygonCollider2D>().sharedMaterial;
					//DestroyImmediate(go.GetComponent<PolygonCollider2D>());
					Undo.DestroyObjectImmediate(go.GetComponent<PolygonCollider2D>());
					//go.AddComponent<PolygonCollider2D>();
					Undo.AddComponent<PolygonCollider2D>(go);
					go.GetComponent<PolygonCollider2D>().isTrigger = isTrigger;
					go.GetComponent<PolygonCollider2D>().sharedMaterial = physMat;
					cleanVertCount += go.GetComponent<PolygonCollider2D>().GetTotalPointCount();
				}
			}

			origVertCount = cleanVertCount;

			// calculate statistics
			if (cleanVertCount>0)
			{
				removedVertPercent = 100-(int)((float)cleanVertCount/(float)origVertCount*100.0f);
			}else{
				removedVertPercent = 0;
			}

			CalculateSelection();

		}


		// add polygoncollider to sprites, without any colliders
		void AddPolygon2DCollider()
		{
			foreach (GameObject go in Selection.gameObjects)
			{
				if (go.GetComponent<SpriteRenderer>()!=null)
				{
					if (go.GetComponent<PolygonCollider2D>()==null &&
					    go.GetComponent<BoxCollider2D>()==null &&
						go.GetComponent<CircleCollider2D>()==null &&
						go.GetComponent<EdgeCollider2D>()==null)
					{
						Undo.AddComponent<PolygonCollider2D>(go);
					}
				}
			}
			CalculateSelection();
		}

		// remove polygoncollider from sprites
		void RemovePolygon2DCollider()
		{
			foreach (GameObject go in Selection.gameObjects)
			{
				if (go.GetComponent<SpriteRenderer>()!=null)
				{
					if (go.GetComponent<PolygonCollider2D>()!=null)
					{
						Undo.DestroyObjectImmediate(go.GetComponent<PolygonCollider2D>());
					}
				}
			}
			CalculateSelection();
		}




		// count objects by type
		void CalculateSelection()
		{
			int spriteCount=0;
			int spriteCollider2DCount=0;
			int spriteColliderPaths=0;
			int spriteSomeCollider2DCount=0;
			int spriteNoCollidersAtAllCount=0;
			int otherCount=0;

//			origVertCount = 0;
//			cleanVertCount = 0;
//			removedVertPercent = 0;
			int vertCount=0;
			
			foreach (GameObject go in Selection.gameObjects)
			{
				if (go.GetComponent<SpriteRenderer>()==null)
				{
					otherCount++;
				}else{ // is sprite

					spriteCount++;

					if (go.GetComponent<PolygonCollider2D>()!=null)
					{
						spriteCollider2DCount++;
						spriteColliderPaths+=go.GetComponent<PolygonCollider2D>().pathCount;
						vertCount += go.GetComponent<PolygonCollider2D>().GetTotalPointCount();
						
					}else{ // is sprite, but no 2D polygon collider
						if 	(go.GetComponent<BoxCollider2D>()==null &&
						     go.GetComponent<CircleCollider2D>()==null &&
						     go.GetComponent<EdgeCollider2D>()==null)
						{
							spriteNoCollidersAtAllCount++;
						}else{
							spriteSomeCollider2DCount++;
						}
						
					}
				}
			}

			// TODO: calculate paths
			selectionString = "Sprites: " + spriteCount + " | PolygonCollider2D(s): " + spriteCollider2DCount + "\n";
			selectionString +="Paths: " + spriteColliderPaths + " | Vertices: " + vertCount + "\n";
			selectionString += "no 2D colliders: " + spriteNoCollidersAtAllCount + " | Other 2DCollider(s): " +spriteSomeCollider2DCount+ "\n";
			selectionString += "Other objects: " + otherCount;

			// calculate statistics
//			origVertCount = cleanVertCount;
//			if (cleanVertCount>0)
//			{
//				removedVertPercent = 100-(int)((float)cleanVertCount/(float)origVertCount*100.0f);
//			}else{
//				removedVertPercent = 0;
//			}

			Repaint();
		}


		// removes paths, only largest is left
		void KeepLargestPath()
		{
			// reset stats
			origVertCount = 0;
			cleanVertCount = 0;
			removedVertPercent = 0;

			foreach (GameObject go in Selection.gameObjects)
			{
				// this gameobject
				if (go.GetComponent<PolygonCollider2D>()!=null)
				{

					PolygonCollider2D pc = go.GetComponent<PolygonCollider2D>();

					// more than 1
					if (pc.pathCount>1)
					{

						// get bounds of each path
						int biggestPath = -1;
						float biggestSize = -1;
						for (int i=0; i<pc.pathCount ;i++)
						{
							origVertCount+=pc.GetPath(i).Length;

							// get bounds
							float xMin=Mathf.Infinity;
							float yMin=Mathf.Infinity;
							float xMax=-Mathf.Infinity;
							float yMax=-Mathf.Infinity;

							// find edges

							for (int j=0; j < pc.GetPath(i).Length; j++) 
							{
								xMin = Mathf.Min(xMin,pc.GetPath(i)[j].x);
								xMax = Mathf.Max(xMax,pc.GetPath(i)[j].x);
								yMin = Mathf.Min(yMin,pc.GetPath(i)[j].y);
								yMax = Mathf.Max(yMax,pc.GetPath(i)[j].y);
							}

//							Vector2 center = new Vector2((xMax+xMin)*0.5f,(yMax+yMin)*0.5f);
							//Vector2 size = new Vector2(xMin,yMax);
							float area = (xMax-xMin)*(yMax-yMin);

							// TODO: calculate polygon area instead?

							//biggestSize = Mathf.Max(biggestSize,size.magnitude);
//							if (size.magnitude>biggestSize)
							if (area>biggestSize)
							{
								biggestSize = area;
								biggestPath = i;
							}
							//Debug.Log ("i:"+i+" : "+area);
						}


						//Debug.Log ("keep: "+biggestPath);

						// assign it
						pc.SetPath (0, pc.GetPath(biggestPath));
//						pc.SetPath (0, pc.GetPath(0));
						pc.pathCount = 1;

						cleanVertCount += pc.GetTotalPointCount();

					}
				} // if pol2D collider
				
			} // for each go

			//CalculateSelection();

			// calculate statistics
			if (cleanVertCount>0)
			{
				removedVertPercent = 100-(int)((float)cleanVertCount/(float)origVertCount*100.0f);
			}else{
				removedVertPercent = 0;
			}


		} // keeplargetspath

		// remove paths less than amount of verts
		void FilterPaths(int filterAmount)
		{
			// reset stats
			origVertCount = 0;
			cleanVertCount = 0;
			removedVertPercent = 0;
			
			foreach (GameObject go in Selection.gameObjects)
			{
				// this gameobject
				if (go.GetComponent<PolygonCollider2D>()!=null)
				{
					PolygonCollider2D pc = go.GetComponent<PolygonCollider2D>();
					
					// have more than 1?
					if (pc.pathCount>1)
					{
						origVertCount = pc.GetTotalPointCount();
						List<int> keep = new List<int>();
						
						// check each path
						for (int i=0; i<pc.pathCount ;i++)
						{
							if (pc.GetPath(i).Length>=filterAmount)
							{
								if (!keep.Contains(i))
								{
									keep.Add (i);
								}
							}
						} // each path

						List<Vector2[]> keepPaths = new List<Vector2[]>();
						for (int i=0;i<keep.Count;i++)
						{
							keepPaths.Add (pc.GetPath(keep[i]));
						}
						for (int i=0;i<keepPaths.Count;i++)
						{
							pc.SetPath (i, keepPaths[i]);
						}

						pc.pathCount = keepPaths.Count;
						cleanVertCount = pc.GetTotalPointCount();
					}
				} // if pol2D collider
			} // for each go

			//CalculateSelection();

			// calculate statistics
			if (cleanVertCount>0)
			{
				removedVertPercent = 100-(int)((float)cleanVertCount/(float)origVertCount*100.0f);
			}else{
				removedVertPercent = 0;
			}
		} // filterpaths

		// removes internal paths (point inside polygon test)
		void RemoveInternalPathsComplex()
		{
			// reset stats
			origVertCount = 0;
			cleanVertCount = 0;
			removedVertPercent = 0;

			foreach (GameObject go in Selection.gameObjects)
			{
				// this gameobject
				if (go.GetComponent<PolygonCollider2D>()!=null)
				{
					PolygonCollider2D pc = go.GetComponent<PolygonCollider2D>();
					origVertCount += pc.GetTotalPointCount();

					// more than 1
					if (pc.pathCount>1)
					{
						List<int> keep = new List<int>();

						// check each path
						for (int i=0; i<pc.pathCount ;i++)
						{
							// compare each path
							for (int j=0; j<pc.pathCount ;j++)
							{
								// if not the same path
								if (i!=j)
								{
									bool isInside=false;

									// compare each point of inner path to outer path
									for (int k=0; k<pc.GetPath(j).Length ;k++)
									{
										// is it inside anything?
										if (IsPointInPolygon( pc.GetPath(i) , pc.GetPath(j)[k] ))
										//if (IsInPolygon(pc.GetPath(i),pc.GetPath(j)[k]))
										{
											if (keep.Contains(j))
											{
												keep.Remove(j);
											}
											isInside = true;
											break;
										}else{
											//isInside = false;
											//break;
										}
									}

									// if we didnt find any verts inside
									if (!isInside)
									{
										// then add to keep paths list
										if (!keep.Contains(j))
										{
											//Debug.Log ("addkeep:"+j+" (inside:"+i);
											keep.Add (j);
										}
									}

								}

							}
						} // each path

						// now take those paths that are for keeps
						List<Vector2[]> keepPaths = new List<Vector2[]>();
						keepPaths.Clear();
						for (int i=0;i<keep.Count;i++)
						{
							//Debug.Log ("i:"+i+" keeping:"+keep[i]);
							keepPaths.Add (pc.GetPath( keep[i] ));
						}

						// TODO: sort by size?

						pc.pathCount = keepPaths.Count;
						//pc.SetPath (0, pc.GetPath(0));

						//pc.pathCount = 1;

						for (int i=0;i<keepPaths.Count;i++)
						{
							pc.SetPath (i, keepPaths[i]);
						}


					} // more than 1 path
					cleanVertCount += pc.GetTotalPointCount();
				} // if pol2D collider
			} // for each go
			
			//CalculateSelection();

			// calculate statistics
			if (cleanVertCount>0)
			{
				removedVertPercent = 100-(int)((float)cleanVertCount/(float)origVertCount*100.0f);
			}else{
				removedVertPercent = 0;
			}


		} // removeinternalpathscomplex


		
		//http://dominoc925.blogspot.fi/2012/02/c-code-snippet-to-determine-if-point-is.html
		private bool IsPointInPolygon(Vector2[] polygon, Vector2 point)
		{
			bool isInside = false;
			for (int i = 0, j = polygon.Length - 1; i < polygon.Length; j = i++)
			{
				if (((polygon[i].y > point.y) != (polygon[j].y > point.y)) &&
				    (point.x < (polygon[j].x - polygon[i].x) * (point.y - polygon[i].y) / (polygon[j].y - polygon[i].y) + polygon[i].x))
				{
					isInside = !isInside;
				}
			}
			return isInside;
		}
		

		//http://www.java2s.com/Code/CSharp/Development-Class/PerpendicularVector2.htm
		Vector2 GetPerpendicular(Vector2 original)
		{
			float x = original.x;
			float y = original.y;
			y = -y;
			return new Vector2(y, x);
		}

		// http://stackoverflow.com/a/16841009
		Vector2 GetCentroid(Vector2[] points)
		{
			float area = 0.0f;
			float Cx = 0.0f;
			float Cy = 0.0f;
			float tmp = 0.0f;
			int k;
			
			for (int i = 0; i <= (points.Length-1); i++)
			{
				k = (i + 1) % ((points.Length-1) + 1);
				tmp = points[i].x * points[k].y - points[k].x * points[i].y;
				area += tmp;
				Cx += (points[i].x + points[k].x) * tmp;
				Cy += (points[i].y + points[k].y) * tmp;
			}
			area *= 0.5f;
			Cx *= 1.0f / (6.0f * area);
			Cy *= 1.0f / (6.0f * area);
			
			return new Vector2(Cx, Cy);
		}


	} // class

} // namespace