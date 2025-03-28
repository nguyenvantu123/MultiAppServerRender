﻿// Copyright (c) Jan Škoruba. All Rights Reserved.
// Licensed under the Apache License, Version 2.0.

namespace BlazorIdentity.Helpers
{
    public class SelectItem
	{
		public SelectItem(string id, string text)
		{
			Id = id;
			Text = text;
		}

		public string Id { get; set; }

		public string Text { get; set; }
	}
}