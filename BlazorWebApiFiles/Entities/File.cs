﻿using BlazorWebApiFiles.Entity._base;
using Finbuckle.MultiTenant;

namespace BlazorWebApi.Files.Entities
{

    [MultiTenant]
    public class FileData : EntityBase
    {

        public string Name { get; set; }

        public string AlternativeText { get; set; }

        public string Caption { get; set; }

        public int? Width { get; set; }

        public int? Height { get; set; }

        public string Format { get; set; }

        public string Hash { get; set; }

        public string Ext { get; set; }

        public string Mime { get; set; }

        public decimal? Size { get; set; }

        public string PreviewUrl { get; set; }

        public string Provider { get; set; }

        public string Provider_Metadata { get; set; }

        public string FolderPath { get; set; }

        public Guid? FolderId { get; set; }

        public Folder Folder { get; set; }
    }
}