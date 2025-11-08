using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class TestingGridTool : SerializedMonoBehaviour
{
    [TableMatrix(HorizontalTitle = "Read Only Matrix", IsReadOnly = true)]
    public string[,] ReadOnlyMatrix = new string[7, 7];
}
