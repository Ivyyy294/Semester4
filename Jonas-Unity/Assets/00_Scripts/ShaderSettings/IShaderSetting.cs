using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IShaderSetting
{
	public void SetMaterialProperties(MaterialPropertyBlock propertyBlock);
}
