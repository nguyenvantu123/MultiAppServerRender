// Copyright (c) Jan Škoruba. All Rights Reserved.
// Licensed under the Apache License, Version 2.0.

using System;
using System.Collections.Generic;
using System.Linq;
using BlazorIdentityApi.Common;

namespace BlazorIdentityApi.Helpers
{
	public class EnumHelpers
	{
		public static List<SelectItem> ToSelectList<T>() where T : struct, IComparable
		{
			var selectItems = Enum.GetValues(typeof(T))
				.Cast<T>()
				.Select(x => new SelectItem(Convert.ToInt16(x).ToString(), x.ToString())).ToList();

			return selectItems;
		}
	}
}