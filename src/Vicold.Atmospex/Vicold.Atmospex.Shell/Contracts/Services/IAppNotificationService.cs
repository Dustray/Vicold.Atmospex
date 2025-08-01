﻿using System.Collections.Specialized;

namespace Vicold.Atmospex.Shell.Contracts.Services;

public interface IAppNotificationService
{
    void Initialize();

    bool Show(string payload);

    NameValueCollection ParseArguments(string arguments);

    void Unregister();
}
