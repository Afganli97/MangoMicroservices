﻿using System;
namespace Mango.Services.ShopingCartAPI.Models.DTOs
{
	public class ResponseDto
	{
		public bool IsSuccess { get; set; } = true;
        public object Result { get; set; }
        public string DisplayMessage { get; set; }
        public List<string> ErrorMessages { get; set; }
	}
}

