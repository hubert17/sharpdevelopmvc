using System;
using AutoMapper;

public class AutoMapperConfig
{
    public static void Initialize()
    {
        Mapper.Initialize((config) =>
        {
            config.CreateMap<string, DateTime>().ConvertUsing(dt => string.IsNullOrWhiteSpace(dt) ? DateTime.MinValue : Convert.ToDateTime(dt));
            config.CreateMap<DateTime, string>().ConvertUsing(dt => dt == DateTime.MinValue ? null : dt.ToString("o"));
            config.CreateMap<string, DateTime?>().ConvertUsing(dt => string.IsNullOrWhiteSpace(dt) ? default(DateTime?) : Convert.ToDateTime(dt));
            config.CreateMap<DateTime?, string>().ConvertUsing(dt => dt == null ? null : dt.Value.ToString("o"));    	                  	   	                  
        });
    }
}