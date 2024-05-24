using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoiseFilterFactory
{
   public static INoiseFilter CreateNoiseFilter (NoiseSettings settings)
	{
		switch (settings.m_filterTyp)
		{
			case NoiseSettings.FilterTyp.Simple:
				return new SimpleNoiseFilter (settings);
			case NoiseSettings.FilterTyp.Ridgig:
				return new RidgidNoiseFilter (settings);
		}

		return null;
	}
}
