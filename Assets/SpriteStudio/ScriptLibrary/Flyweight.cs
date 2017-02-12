using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Library_SpriteStudio_Flyweight
{
	[System.Serializable]
	public class Flyweight
	{
		[SerializeField]
		List<Vector2> vector2s = new List<Vector2>();
		public List<Vector2> Vector2s
		{
			get
			{
				return vector2s;
			}
		}

		[SerializeField]
		List<Library_SpriteStudio.Data.AttributeCoordinateMeshFix> coordinateMeshFixes = new List<Library_SpriteStudio.Data.AttributeCoordinateMeshFix>();
		public List<Library_SpriteStudio.Data.AttributeCoordinateMeshFix> CoordinateMeshFixes
		{
			get
			{
				return coordinateMeshFixes;
			}
		}

		[SerializeField]
		List<Library_SpriteStudio.Data.AttributeColorBlendMeshFix> colorMeshFixes = new List<Library_SpriteStudio.Data.AttributeColorBlendMeshFix>();
		public List<Library_SpriteStudio.Data.AttributeColorBlendMeshFix> ColorMeshFixes
		{
			get
			{
				return colorMeshFixes;
			}
		}

		[SerializeField]
		List<Library_SpriteStudio.Data.AttributeUVMeshFix> uvMeshFixes = new List<Library_SpriteStudio.Data.AttributeUVMeshFix>();
		public List<Library_SpriteStudio.Data.AttributeUVMeshFix> UVMeshFixes
		{
			get
			{
				return uvMeshFixes;
			}
		}
	}
}
