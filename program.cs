services.AddDistributedMemoryCache();
services.AddScoped(typeof(ICashService<>), typeof(CashService<>));
