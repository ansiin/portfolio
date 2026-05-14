const keys = {
    apiBaseUrl: "investing-tracker.api-base-url",
    jwt: "investing-tracker.jwt",
    refreshToken: "investing-tracker.refresh-token",
    language: "investing-tracker.language"
};

const translations = {
    en: {
        standaloneFrontend: "Standalone frontend",
        appTitle: "Investing Tracker",
        appLead: "Minimal JavaScript client for the same versioned REST API.",
        connection: "Connection",
        apiBaseUrl: "API base URL",
        language: "Language",
        saveApiBase: "Save Settings",
        login: "Login",
        email: "Email",
        password: "Password",
        refreshToken: "Refresh Token",
        logout: "Logout",
        register: "Register",
        firstName: "First name",
        lastName: "Last name",
        createAccount: "Create Account",
        session: "Session",
        status: "Status",
        jwt: "JWT",
        refresh: "Refresh",
        apiDriven: "API-driven",
        dashboard: "Dashboard",
        dashboardLead: "This client uses JWT, refresh token rotation and CRUD calls against the ASP.NET Core backend.",
        reloadData: "Reload Data",
        portfolios: "Portfolios",
        activeAssets: "Active Assets",
        marketValue: "Market Value",
        unrealizedPl: "Unrealized P/L",
        apiPortfolios: "API: Portfolios",
        name: "Name",
        baseCurrency: "Base currency",
        archived: "Archived",
        actions: "Actions",
        apiDashboard: "API: Dashboard",
        timeline: "Timeline",
        period: "Period",
        netAmount: "Net Amount",
        apiAssets: "API: Assets",
        assets: "Assets",
        portfolio: "Portfolio",
        symbol: "Symbol",
        assetType: "Asset type",
        currency: "Currency",
        exchange: "Exchange",
        marketDataProvider: "Market data provider",
        active: "Active",
        asset: "Asset",
        type: "Type",
        apiTransactions: "API: Transactions",
        transactions: "Transactions",
        executedAt: "Executed at",
        quantity: "Quantity",
        unitPrice: "Unit price",
        totalAmount: "Total amount",
        description: "Description",
        singleFeeType: "Single fee type",
        singleFeeAmount: "Single fee amount",
        date: "Date",
        total: "Total",
        fees: "Fees",
        allocation: "Allocation",
        costBasis: "Cost Basis",
        latestPrice: "Latest Price",
        cancel: "Cancel",
        edit: "Edit",
        delete: "Delete",
        yes: "Yes",
        no: "No",
        buy: "Buy",
        sell: "Sell",
        deposit: "Deposit",
        withdrawal: "Withdrawal",
        dividend: "Dividend",
        createPortfolio: "Create Portfolio",
        updatePortfolio: "Update Portfolio",
        createAsset: "Create Asset",
        updateAsset: "Update Asset",
        createTransaction: "Create Transaction",
        updateTransaction: "Update Transaction",
        selectCurrency: "Select currency",
        selectPortfolio: "Select portfolio",
        selectAssetType: "Select asset type",
        optional: "Optional",
        signedIn: "Signed in",
        signedOut: "Signed out",
        present: "Present",
        missing: "Missing",
        noDataYet: "No data yet.",
        settingsSaved: "Settings saved.",
        loginSuccessful: "Login successful.",
        accountCreated: "Account created and signed in.",
        sessionRefreshed: "Session refreshed.",
        loggedOut: "Logged out.",
        signInFirst: "Sign in first.",
        portfolioCreated: "Portfolio created.",
        portfolioUpdated: "Portfolio updated.",
        portfolioDeleted: "Portfolio deleted.",
        assetCreated: "Asset created.",
        assetUpdated: "Asset updated.",
        assetDeleted: "Asset deleted.",
        transactionCreated: "Transaction created.",
        transactionUpdated: "Transaction updated.",
        transactionDeleted: "Transaction deleted.",
        confirmDeletePortfolio: "Delete portfolio \"{name}\"?",
        confirmDeleteAsset: "Delete/deactivate asset \"{name}\"?",
        confirmDeleteTransaction: "Delete transaction?",
        mainPortfolioPlaceholder: "Main Portfolio",
        assetNamePlaceholder: "Apple Inc.",
        optionalNote: "Optional note",
        brokerPlaceholder: "Broker"
    },
    et: {
        standaloneFrontend: "Eraldi frontend",
        appTitle: "Investeeringute jälgija",
        appLead: "Lihtne JavaScript klient sama versioneeritud REST API jaoks.",
        connection: "Ühendus",
        apiBaseUrl: "API baas-URL",
        language: "Keel",
        saveApiBase: "Salvesta seaded",
        login: "Logi sisse",
        email: "E-post",
        password: "Parool",
        refreshToken: "Uuenda tokenit",
        logout: "Logi välja",
        register: "Loo konto",
        firstName: "Eesnimi",
        lastName: "Perekonnanimi",
        createAccount: "Loo konto",
        session: "Sessioon",
        status: "Staatus",
        jwt: "JWT",
        refresh: "Refresh",
        apiDriven: "API-põhine",
        dashboard: "Töölaud",
        dashboardLead: "See klient kasutab JWT-d, refresh tokeni rotatsiooni ja CRUD päringuid ASP.NET Core backendi vastu.",
        reloadData: "Lae andmed uuesti",
        portfolios: "Portfellid",
        activeAssets: "Aktiivsed varad",
        marketValue: "Turuväärtus",
        unrealizedPl: "Realiseerimata K/K",
        apiPortfolios: "API: Portfellid",
        name: "Nimi",
        baseCurrency: "Baasvaluuta",
        archived: "Arhiveeritud",
        actions: "Tegevused",
        apiDashboard: "API: Töölaud",
        timeline: "Ajajoon",
        period: "Periood",
        netAmount: "Netosumma",
        apiAssets: "API: Varad",
        assets: "Varad",
        portfolio: "Portfell",
        symbol: "Sümbol",
        assetType: "Varaklass",
        currency: "Valuuta",
        exchange: "Börs",
        marketDataProvider: "Turuhinna pakkuja",
        active: "Aktiivne",
        asset: "Vara",
        type: "Tüüp",
        apiTransactions: "API: Tehingud",
        transactions: "Tehingud",
        executedAt: "Teostatud",
        quantity: "Kogus",
        unitPrice: "Ühikuhind",
        totalAmount: "Kogusumma",
        description: "Kirjeldus",
        singleFeeType: "Ühe tasu tüüp",
        singleFeeAmount: "Ühe tasu summa",
        date: "Kuupäev",
        total: "Kokku",
        fees: "Tasud",
        allocation: "Jaotus",
        costBasis: "Soetusmaksumus",
        latestPrice: "Viimane hind",
        cancel: "Tühista",
        edit: "Muuda",
        delete: "Kustuta",
        yes: "Jah",
        no: "Ei",
        buy: "Ost",
        sell: "Müük",
        deposit: "Sissemakse",
        withdrawal: "Väljamakse",
        dividend: "Dividend",
        createPortfolio: "Loo portfell",
        updatePortfolio: "Uuenda portfelli",
        createAsset: "Loo vara",
        updateAsset: "Uuenda vara",
        createTransaction: "Loo tehing",
        updateTransaction: "Uuenda tehingut",
        selectCurrency: "Vali valuuta",
        selectPortfolio: "Vali portfell",
        selectAssetType: "Vali varaklass",
        optional: "Valikuline",
        signedIn: "Sisse logitud",
        signedOut: "Välja logitud",
        present: "Olemas",
        missing: "Puudub",
        noDataYet: "Andmeid veel ei ole.",
        settingsSaved: "Seaded salvestatud.",
        loginSuccessful: "Sisselogimine õnnestus.",
        accountCreated: "Konto loodi ja kasutaja logiti sisse.",
        sessionRefreshed: "Sessioon uuendatud.",
        loggedOut: "Välja logitud.",
        signInFirst: "Logi enne sisse.",
        portfolioCreated: "Portfell loodud.",
        portfolioUpdated: "Portfell uuendatud.",
        portfolioDeleted: "Portfell kustutatud.",
        assetCreated: "Vara loodud.",
        assetUpdated: "Vara uuendatud.",
        assetDeleted: "Vara kustutatud.",
        transactionCreated: "Tehing loodud.",
        transactionUpdated: "Tehing uuendatud.",
        transactionDeleted: "Tehing kustutatud.",
        confirmDeletePortfolio: "Kas kustutada portfell \"{name}\"?",
        confirmDeleteAsset: "Kas kustutada/deaktiveerida vara \"{name}\"?",
        confirmDeleteTransaction: "Kas kustutada tehing?",
        mainPortfolioPlaceholder: "Põhiportfell",
        assetNamePlaceholder: "Apple Inc.",
        optionalNote: "Valikuline märkus",
        brokerPlaceholder: "Maakler",
        settingsHint: "Deploy keskkonnas võetakse õige API host automaatselt. Muuda seda välja ainult siis, kui testid teist backendit.",
        demoCredentialsHint: "Demo kasutaja: user@taltech.ee / Kala.12345",
        registerHint: "Loo uus konto ja klient logitakse kohe sisse.",
        apiReturnedHtml: "API tagastas HTML vastuse. Ava Settings ja kontrolli, et API baas-URL oleks https://ansiin-investing-api.proxy.itcollege.ee.",
        connectionIssue: "Ühendus API-ga ebaõnnestus. Kontrolli API baas-URL-i ja proovi uuesti."
    },
    en: {
        standaloneFrontend: "Standalone frontend",
        appTitle: "Investing Tracker",
        appLead: "Minimal JavaScript client for the same versioned REST API.",
        connection: "Connection",
        apiBaseUrl: "API base URL",
        language: "Language",
        saveApiBase: "Save Settings",
        login: "Login",
        email: "Email",
        password: "Password",
        refreshToken: "Refresh Token",
        logout: "Logout",
        register: "Register",
        firstName: "First name",
        lastName: "Last name",
        createAccount: "Create Account",
        session: "Session",
        status: "Status",
        jwt: "JWT",
        refresh: "Refresh",
        apiDriven: "API-driven",
        dashboard: "Dashboard",
        dashboardLead: "This client uses JWT, refresh token rotation and CRUD calls against the ASP.NET Core backend.",
        reloadData: "Reload Data",
        portfolios: "Portfolios",
        activeAssets: "Active Assets",
        marketValue: "Market Value",
        unrealizedPl: "Unrealized P/L",
        apiPortfolios: "API: Portfolios",
        name: "Name",
        baseCurrency: "Base currency",
        archived: "Archived",
        actions: "Actions",
        apiDashboard: "API: Dashboard",
        timeline: "Timeline",
        period: "Period",
        netAmount: "Net Amount",
        apiAssets: "API: Assets",
        assets: "Assets",
        portfolio: "Portfolio",
        symbol: "Symbol",
        assetType: "Asset type",
        currency: "Currency",
        exchange: "Exchange",
        marketDataProvider: "Market data provider",
        active: "Active",
        asset: "Asset",
        type: "Type",
        apiTransactions: "API: Transactions",
        transactions: "Transactions",
        executedAt: "Executed at",
        quantity: "Quantity",
        unitPrice: "Unit price",
        totalAmount: "Total amount",
        description: "Description",
        singleFeeType: "Single fee type",
        singleFeeAmount: "Single fee amount",
        date: "Date",
        total: "Total",
        fees: "Fees",
        allocation: "Allocation",
        costBasis: "Cost Basis",
        latestPrice: "Latest Price",
        cancel: "Cancel",
        edit: "Edit",
        delete: "Delete",
        yes: "Yes",
        no: "No",
        buy: "Buy",
        sell: "Sell",
        deposit: "Deposit",
        withdrawal: "Withdrawal",
        dividend: "Dividend",
        createPortfolio: "Create Portfolio",
        updatePortfolio: "Update Portfolio",
        createAsset: "Create Asset",
        updateAsset: "Update Asset",
        createTransaction: "Create Transaction",
        updateTransaction: "Update Transaction",
        selectCurrency: "Select currency",
        selectPortfolio: "Select portfolio",
        selectAssetType: "Select asset type",
        optional: "Optional",
        signedIn: "Signed in",
        signedOut: "Signed out",
        present: "Present",
        missing: "Missing",
        noDataYet: "No data yet.",
        settingsSaved: "Settings saved.",
        loginSuccessful: "Login successful.",
        accountCreated: "Account created and signed in.",
        sessionRefreshed: "Session refreshed.",
        loggedOut: "Logged out.",
        signInFirst: "Sign in first.",
        portfolioCreated: "Portfolio created.",
        portfolioUpdated: "Portfolio updated.",
        portfolioDeleted: "Portfolio deleted.",
        assetCreated: "Asset created.",
        assetUpdated: "Asset updated.",
        assetDeleted: "Asset deleted.",
        transactionCreated: "Transaction created.",
        transactionUpdated: "Transaction updated.",
        transactionDeleted: "Transaction deleted.",
        confirmDeletePortfolio: "Delete portfolio \"{name}\"?",
        confirmDeleteAsset: "Delete/deactivate asset \"{name}\"?",
        confirmDeleteTransaction: "Delete transaction?",
        mainPortfolioPlaceholder: "Main Portfolio",
        assetNamePlaceholder: "Apple Inc.",
        optionalNote: "Optional note",
        brokerPlaceholder: "Broker",
        settingsHint: "On the deployed site the correct API host is applied automatically. Only change this when testing another backend.",
        demoCredentialsHint: "Demo account: user@taltech.ee / Kala.12345",
        registerHint: "Create a new account and the client signs you in immediately.",
        apiReturnedHtml: "The API returned HTML instead of JSON. Open Settings and make sure the API base URL is https://ansiin-investing-api.proxy.itcollege.ee.",
        connectionIssue: "Could not reach the API. Check the API base URL and try again.",
        authLead: "Track portfolios, assets and transactions in a cleaner standalone workspace.",
        workspaceLabel: "Personal investing workspace",
        authTitle: "Sign in to your investing workspace",
        authFeatureOverview: "Live portfolio overview",
        authFeatureActions: "Fast CRUD workflows",
        authFeatureSecurity: "JWT + refresh auth",
        quickActions: "Quick actions",
        openPortfolios: "Open portfolios",
        openAssets: "Open assets",
        openTransactions: "Open transactions",
        latestPeriod: "Latest period",
        topPosition: "Top position",
        strongestMover: "Strongest mover",
        recentActivity: "Recent activity",
        connectionDesk: "Connection desk",
        sessionControls: "Session controls",
        trendChart: "Trend chart",
        allocationChart: "Allocation chart"
    }
};

const translationOverrides = {
    en: {
        authLead: "Track portfolios, assets and transactions in a cleaner standalone workspace.",
        workspaceLabel: "Personal investing workspace",
        authTitle: "Sign in to your investing workspace",
        authFeatureOverview: "Live portfolio overview",
        authFeatureActions: "Fast CRUD workflows",
        authFeatureSecurity: "JWT + refresh auth",
        quickActions: "Quick actions",
        openPortfolios: "Open portfolios",
        openAssets: "Open assets",
        openTransactions: "Open transactions",
        openSettings: "Open settings",
        openDashboard: "Open dashboard",
        refreshData: "Refresh data",
        sessionExpired: "Your session expired. Please sign in again.",
        latestPeriod: "Latest period",
        topPosition: "Top position",
        strongestMover: "Strongest mover",
        recentActivity: "Recent activity",
        connectionDesk: "Connection desk",
        sessionControls: "Session controls",
        trendChart: "Trend chart",
        allocationChart: "Allocation chart",
        navDashboardHint: "Overview and charts",
        navPortfoliosHint: "Create and manage containers",
        navAssetsHint: "Track positions and symbols",
        navTransactionsHint: "CRUD for trading activity",
        navAccessHint: "Session and API settings",
        portfoliosLead: "Keep base currencies and archive state readable in one place.",
        assetsLead: "Maintain symbols, providers and activation state with less friction.",
        transactionsLead: "Capture buys, sells, deposits and dividends from one focused form.",
        connectionLead: "Manage API host, language and session state without leaving the workspace.",
        accessHelp: "Use this page for connection tuning and session maintenance. The main operational screens stay focused on portfolios, assets and transactions."
    },
    et: {
        appTitle: "Investeeringute jälgija",
        connection: "Ühendus",
        logout: "Logi välja",
        apiDriven: "API-põhine",
        dashboard: "Töölaud",
        dashboardLead: "See klient kasutab JWT-d, refresh tokeni rotatsiooni ja CRUD päringuid ASP.NET Core backendi vastu.",
        marketValue: "Turuväärtus",
        apiDashboard: "API: Töölaud",
        symbol: "Sümbol",
        exchange: "Börs",
        type: "Tüüp",
        unitPrice: "Ühikuhind",
        cancel: "Tühista",
        signedOut: "Välja logitud",
        loginSuccessful: "Sisselogimine õnnestus.",
        optionalNote: "Valikuline märkus",
        settingsHint: "Deploy keskkonnas võetakse õige API host automaatselt. Muuda seda välja ainult siis, kui testid teist backendit.",
        connectionIssue: "Ühendus API-ga ebaõnnestus. Kontrolli API baas-URL-i ja proovi uuesti.",
        authLead: "Jälgi portfelle, varasid ja tehinguid puhtamas eraldi töökeskkonnas.",
        workspaceLabel: "Isiklik investeerimise töölaud",
        authTitle: "Logi sisse oma investeerimise töölauale",
        authFeatureOverview: "Reaalajas portfelli ülevaade",
        authFeatureActions: "Kiired CRUD-vood",
        authFeatureSecurity: "JWT + refresh autentimine",
        quickActions: "Kiirvalikud",
        openPortfolios: "Ava portfellid",
        openAssets: "Ava varad",
        openTransactions: "Ava tehingud",
        openSettings: "Ava seaded",
        openDashboard: "Ava töölaud",
        refreshData: "Värskenda andmeid",
        sessionExpired: "Sessioon aegus. Logi uuesti sisse.",
        latestPeriod: "Viimane periood",
        topPosition: "Suurim positsioon",
        strongestMover: "Parim liikuja",
        recentActivity: "Viimased tegevused",
        connectionDesk: "Ühenduse keskus",
        sessionControls: "Sessiooni haldus",
        trendChart: "Trendijoonis",
        allocationChart: "Jaotuse graafik",
        navDashboardHint: "Ülevaade ja graafikud",
        navPortfoliosHint: "Loo ja halda portfelle",
        navAssetsHint: "Halda positsioone ja sümboleid",
        navTransactionsHint: "Tehingute töölaud",
        navAccessHint: "Sessioon ja API seaded",
        portfoliosLead: "Hoia baasvaluutad ja arhiveerimise seis ühes loetavas vaates.",
        assetsLead: "Halda sümboleid, pakkujaid ja aktiivsuse staatust vähemate klikkidega.",
        transactionsLead: "Sisesta ostud, müügid, sissemaksed ja dividendid ühest fokusseeritud vormist.",
        connectionLead: "Halda API hosti, keelt ja sessiooni ilma tööruumist lahkumata.",
        accessHelp: "Kasuta seda vaadet ühenduse ja sessiooni seadistamiseks. Põhivood jäävad portfellide, varade ja tehingute lehtedele."
    }
};

Object.entries(translationOverrides).forEach(([language, values]) => {
    translations[language] = { ...(translations[language] || {}), ...values };
});

const fallbackText = {
    authLead: "Modern investing workspace",
    workspaceLabel: "Investing workspace",
    authTitle: "Sign in",
    authFeatureOverview: "Portfolio overview",
    authFeatureActions: "Fast workflows",
    authFeatureSecurity: "Secure session",
    quickActions: "Quick actions",
    openPortfolios: "Open portfolios",
    openAssets: "Open assets",
    openTransactions: "Open transactions",
    openSettings: "Open settings",
    openDashboard: "Open dashboard",
    refreshData: "Refresh data",
    sessionExpired: "Your session expired. Please sign in again.",
    latestPeriod: "Latest period",
    topPosition: "Top position",
    strongestMover: "Strongest mover",
    recentActivity: "Recent activity",
    connectionDesk: "Connection desk",
    sessionControls: "Session controls",
    trendChart: "Trend chart",
    allocationChart: "Allocation chart",
    navDashboardHint: "Overview and charts",
    navPortfoliosHint: "Create and manage containers",
    navAssetsHint: "Track positions and symbols",
    navTransactionsHint: "Transactions and activity",
    navAccessHint: "Connection and settings",
    portfoliosLead: "Manage portfolio setup.",
    assetsLead: "Manage assets and providers.",
    transactionsLead: "Create and review transactions.",
    connectionLead: "Manage API and session settings.",
    accessHelp: "Use this page to manage the connection and session."
};

const state = {
    language: normalizeLanguage(localStorage.getItem(keys.language) || document.documentElement.lang || "en"),
    apiBaseUrl: resolveInitialApiBaseUrl(),
    jwt: localStorage.getItem(keys.jwt),
    refreshToken: localStorage.getItem(keys.refreshToken),
    isAuthenticated: false,
    page: resolvePageFromHash(),
    lookups: { currencies: [], assetTypes: [], exchanges: [], providers: [] },
    portfolios: [],
    assets: [],
    transactions: [],
    summary: null,
    allocation: [],
    timeline: [],
    editing: { portfolioId: null, assetId: null, transactionId: null }
};

const el = {};

document.addEventListener("DOMContentLoaded", () => {
    cache();
    bind();
    syncSettings();
    syncPage();
    applyLanguage();
    resetPortfolioForm();
    resetAssetForm();
    resetTransactionForm();
    syncSession();

    if (state.jwt && state.refreshToken) {
        hydrate().catch((error) => handleSessionFailure(error, { silent: true }));
    } else {
        clearSession();
    }
});

function cache() {
    document.querySelectorAll("[id]").forEach((item) => {
        el[item.id.replace(/-([a-z])/g, (_, c) => c.toUpperCase())] = item;
    });
}

function bind() {
    document.querySelectorAll("[data-nav-page]").forEach((button) => {
        button.addEventListener("click", () => setPage(button.dataset.navPage || "dashboard"));
    });
    window.addEventListener("hashchange", () => {
        state.page = resolvePageFromHash();
        syncPage();
    });

    el.settingsForm.addEventListener("submit", saveSettings);
    el.languageSelect.addEventListener("change", handleLanguageChange);
    el.loginForm.addEventListener("submit", login);
    el.registerForm.addEventListener("submit", register);
    el.refreshSession.addEventListener("click", refreshSession);
    el.logoutButton.addEventListener("click", logout);
    el.reloadAll.addEventListener("click", () => hydrate().catch((error) => message(error.message, true)));
    el.dashboardRefreshButton?.addEventListener("click", () => hydrate().catch((error) => message(error.message, true)));

    el.portfolioForm.addEventListener("submit", savePortfolio);
    el.portfolioCancel.addEventListener("click", resetPortfolioForm);
    el.portfolioRows.addEventListener("click", handlePortfolioRowAction);

    el.assetForm.addEventListener("submit", saveAsset);
    el.assetCancel.addEventListener("click", resetAssetForm);
    el.assetRows.addEventListener("click", handleAssetRowAction);

    el.transactionForm.addEventListener("submit", saveTransaction);
    el.transactionCancel.addEventListener("click", resetTransactionForm);
    el.transactionPortfolio.addEventListener("change", renderTransactionAssetOptions);
    el.transactionRows.addEventListener("click", handleTransactionRowAction);
}

function syncSettings() {
    state.apiBaseUrl = normalizeBaseUrl(state.apiBaseUrl) || resolveInitialApiBaseUrl();
    localStorage.setItem(keys.apiBaseUrl, state.apiBaseUrl);
    localStorage.setItem(keys.language, state.language);
    el.apiBaseUrl.value = state.apiBaseUrl;
    el.languageSelect.value = state.language;

    const label = formatApiBaseLabel(state.apiBaseUrl);
    if (el.topbarApiBase) el.topbarApiBase.textContent = label;
    if (el.sidebarApiHost) el.sidebarApiHost.textContent = label;
    if (el.dashboardApiHost) el.dashboardApiHost.textContent = label;
}

function handleLanguageChange() {
    state.language = normalizeLanguage(el.languageSelect.value);
    localStorage.setItem(keys.language, state.language);
    applyLanguage();
}

function saveSettings(event) {
    event.preventDefault();
    const inputBaseUrl = normalizeBaseUrl(el.apiBaseUrl.value);
    if (window.location.hostname.toLowerCase() === "ansiin-investing.proxy.itcollege.ee" &&
        (!inputBaseUrl || inputBaseUrl === window.location.origin || inputBaseUrl === "https://ansiin-investing.proxy.itcollege.ee")) {
        state.apiBaseUrl = "https://ansiin-investing-api.proxy.itcollege.ee";
    } else {
        state.apiBaseUrl = inputBaseUrl || resolveInitialApiBaseUrl();
    }
    state.language = normalizeLanguage(el.languageSelect.value);
    syncSettings();
    applyLanguage();
    message(t("settingsSaved"), false, true);
}

function resolvePageFromHash() {
    const page = window.location.hash.replace(/^#/, "").trim().toLowerCase();
    return ["dashboard", "portfolios", "assets", "transactions", "access"].includes(page)
        ? page
        : "dashboard";
}

function setPage(page, pushHash = true) {
    state.page = ["dashboard", "portfolios", "assets", "transactions", "access"].includes(page)
        ? page
        : "dashboard";

    if (pushHash) {
        const nextHash = `#${state.page}`;
        if (window.location.hash !== nextHash) {
            window.location.hash = nextHash;
        }
    }

    syncPage();
}

function syncPage() {
    document.querySelectorAll("[data-page]").forEach((section) => {
        section.classList.toggle("is-active", section.dataset.page === state.page);
    });

    document.querySelectorAll("[data-nav-page]").forEach((button) => {
        const isActive = button.dataset.navPage === state.page;
        button.classList.toggle("is-active", isActive);
        button.setAttribute("aria-current", isActive ? "page" : "false");
    });
}

function resolveInitialApiBaseUrl() {
    const configured = normalizeBaseUrl(window.INVESTING_TRACKER_CONFIG?.apiBaseUrl || "");
    const stored = normalizeBaseUrl(localStorage.getItem(keys.apiBaseUrl) || "");
    const host = window.location.hostname.toLowerCase();
    const currentOrigin = normalizeBaseUrl(window.location.origin);

    if (host === "ansiin-investing.proxy.itcollege.ee") {
        return "https://ansiin-investing-api.proxy.itcollege.ee";
    }

    if (host === "ansiin-investing-api.proxy.itcollege.ee") {
        return normalizeBaseUrl(window.location.origin);
    }

    if (host === "192.168.181.136" && window.location.port === "83") {
        return "http://192.168.181.136:82";
    }

    if (stored === currentOrigin || stored === "https://ansiin-investing.proxy.itcollege.ee") {
        return "https://ansiin-investing-api.proxy.itcollege.ee";
    }

    if (stored === "http://192.168.181.136:83") {
        return "http://192.168.181.136:82";
    }

    if (stored.includes("ansiin.proxy.itcollege.ee")) {
        return "https://ansiin-investing-api.proxy.itcollege.ee";
    }

    return stored || configured || "https://localhost:7192";
}

function applyLanguage() {
    document.documentElement.lang = state.language;
    document.querySelectorAll("[data-i18n]").forEach((node) => {
        node.textContent = t(node.dataset.i18n);
    });

    el.portfolioName.placeholder = t("mainPortfolioPlaceholder");
    el.assetName.placeholder = t("assetNamePlaceholder");
    el.transactionDescription.placeholder = t("optionalNote");
    el.transactionFeeType.placeholder = t("brokerPlaceholder");

    applyTransactionTypeLabels();
    syncSession();
    renderSummary();
    renderFormLabels();
    renderPortfolioOptions();
    renderAssetLookupOptions();
    renderTransactionAssetOptions();
    renderTables();
    syncPage();
}

async function login(event) {
    event.preventDefault();

    try {
        const data = await api("/api/v1/identity/account/login", "POST", {
            email: el.email.value.trim(),
            password: el.password.value
        }, false);

        setSession(data.jwt, data.refreshToken);
        setPage("dashboard");
        await hydrate();
        message(t("loginSuccessful"), false, true);
    } catch (error) {
        message(error.message, true);
    }
}

async function register(event) {
    event.preventDefault();

    try {
        const email = el.registerEmail.value.trim();
        const password = el.registerPassword.value;
        const firstname = el.registerFirstname.value.trim() || "New";
        const lastname = el.registerLastname.value.trim() || "User";

        const data = await api("/api/v1/identity/account/register", "POST", {
            email,
            password,
            firstname,
            lastname
        }, false);

        setSession(data.jwt, data.refreshToken);
        setPage("dashboard");
        el.email.value = email;
        el.password.value = password;
        el.registerForm.reset();
        await hydrate();
        message(t("accountCreated"), false, true);
    } catch (error) {
        message(error.message, true);
    }
}

async function refreshSession() {
    try {
        await refreshTokens();
        await hydrate();
        message(t("sessionRefreshed"), false, true);
    } catch (error) {
        handleSessionFailure(error);
    }
}

async function logout() {
    try {
        if (state.jwt && state.refreshToken) {
            await api("/api/v1/identity/account/logout", "POST", { refreshToken: state.refreshToken }, false);
        }
    } catch (error) {
        console.warn(error);
    } finally {
        clearSession();
        setPage("access");
        state.summary = null;
        state.allocation = [];
        state.timeline = [];
        state.portfolios = [];
        state.assets = [];
        state.transactions = [];
        clearTables();
        resetPortfolioForm();
        resetAssetForm();
        resetTransactionForm();
        message(t("loggedOut"), false, true);
    }
}

async function hydrate() {
    if (!state.jwt) {
        throw new Error(t("signInFirst"));
    }

    await ensureLookupsLoaded();

    const [summary, allocation, timeline, portfolios, assets, transactions] = await Promise.all([
        api("/api/v1/Dashboard/summary"),
        api("/api/v1/Dashboard/allocation"),
        api("/api/v1/Dashboard/timeline"),
        api("/api/v1/Portfolios"),
        api("/api/v1/Assets"),
        api("/api/v1/Transactions")
    ]);

    state.summary = summary;
    state.allocation = allocation;
    state.timeline = timeline;
    state.portfolios = portfolios;
    state.assets = assets;
    state.transactions = transactions;
    state.isAuthenticated = true;

    renderSummary();
    renderPortfolioOptions();
    renderAssetLookupOptions();
    renderTransactionAssetOptions();
    renderTables();
    syncSession();
}

async function ensureLookupsLoaded() {
    if (state.lookups.currencies.length) {
        return;
    }

    const [currencies, assetTypes, exchanges, providers] = await Promise.all([
        api("/api/v1/Currencies"),
        api("/api/v1/AssetTypes"),
        api("/api/v1/Exchanges"),
        api("/api/v1/MarketDataProviders")
    ]);

    state.lookups = { currencies, assetTypes, exchanges, providers };
}

function renderSummary() {
    if (!state.summary) {
        el.metricPortfolios.textContent = "0";
        el.metricAssets.textContent = "0";
        el.metricMarketValue.textContent = "0";
        el.metricUnrealized.textContent = "0";
        el.metricUnrealized.className = "metric-value";
        renderDashboardExperience();
        return;
    }

    el.metricPortfolios.textContent = state.summary.portfolioCount;
    el.metricAssets.textContent = state.summary.activeAssetCount;
    el.metricMarketValue.textContent = number(state.summary.totalMarketValue);
    el.metricUnrealized.textContent = number(state.summary.totalUnrealizedProfit);
    el.metricUnrealized.className = `metric-value ${state.summary.totalUnrealizedProfit >= 0 ? "positive" : "negative"}`;
    renderDashboardExperience();
}

function renderPortfolioOptions() {
    if (!el.portfolioBaseCurrency) {
        return;
    }

    const portfolioOptions = state.portfolios.map((item) => ({
        value: item.id,
        label: `${item.name} (${item.baseCurrencyCode})`
    }));

    setOptions(el.portfolioBaseCurrency, state.lookups.currencies.map(mapLookup), {
        emptyLabel: t("selectCurrency"),
        selectedValue: state.editing.portfolioId
            ? state.portfolios.find((item) => item.id === state.editing.portfolioId)?.baseCurrencyId
            : el.portfolioBaseCurrency.value
    });

    setOptions(el.assetPortfolio, portfolioOptions, {
        emptyLabel: t("selectPortfolio"),
        selectedValue: state.editing.assetId
            ? state.assets.find((item) => item.id === state.editing.assetId)?.portfolioId
            : el.assetPortfolio.value
    });

    setOptions(el.transactionPortfolio, portfolioOptions, {
        emptyLabel: t("selectPortfolio"),
        selectedValue: state.editing.transactionId
            ? state.transactions.find((item) => item.id === state.editing.transactionId)?.portfolioId
            : el.transactionPortfolio.value
    });
}

function renderAssetLookupOptions() {
    if (!el.assetType) {
        return;
    }

    const current = state.editing.assetId
        ? state.assets.find((item) => item.id === state.editing.assetId)
        : null;

    setOptions(el.assetType, state.lookups.assetTypes.map(mapLookup), {
        emptyLabel: t("selectAssetType"),
        selectedValue: current?.assetTypeId || el.assetType.value
    });

    setOptions(el.assetCurrency, state.lookups.currencies.map(mapLookup), {
        emptyLabel: t("selectCurrency"),
        selectedValue: current?.currencyId || el.assetCurrency.value
    });

    setOptions(el.assetExchange, state.lookups.exchanges.map(mapLookup), {
        allowEmpty: true,
        emptyLabel: t("optional"),
        selectedValue: current?.exchangeId || el.assetExchange.value
    });

    setOptions(el.assetProvider, state.lookups.providers.map(mapLookup), {
        allowEmpty: true,
        emptyLabel: t("optional"),
        selectedValue: current?.marketDataProviderId || el.assetProvider.value
    });
}

function renderTransactionAssetOptions() {
    if (!el.transactionAsset) {
        return;
    }

    const portfolioId = el.transactionPortfolio.value;
    const filteredAssets = portfolioId
        ? state.assets.filter((item) => item.portfolioId === portfolioId)
        : state.assets;

    const current = state.editing.transactionId
        ? state.transactions.find((item) => item.id === state.editing.transactionId)
        : null;

    setOptions(el.transactionAsset, filteredAssets.map((item) => ({
        value: item.id,
        label: item.symbol ? `${item.name} (${item.symbol})` : item.name
    })), {
        allowEmpty: true,
        emptyLabel: t("optional"),
        selectedValue: current?.assetId || el.transactionAsset.value
    });
}

function renderTables() {
    renderTable(el.portfolioRows, state.portfolios, 4, (item) => `
        <tr>
            <td>${safe(item.name)}</td>
            <td>${safe(item.baseCurrencyCode)}</td>
            <td>${item.isArchived ? t("yes") : t("no")}</td>
            <td class="table-actions">
                <button type="button" class="button button-small button-secondary" data-entity="portfolio" data-action="edit" data-id="${item.id}">${t("edit")}</button>
                <button type="button" class="button button-small button-danger" data-entity="portfolio" data-action="delete" data-id="${item.id}">${t("delete")}</button>
            </td>
        </tr>`);

    renderTable(el.assetRows, state.assets, 6, (item) => `
        <tr>
            <td>${safe(item.name)}${item.symbol ? ` <span class="muted">(${safe(item.symbol)})</span>` : ""}</td>
            <td>${safe(item.portfolioName)}</td>
            <td>${safe(item.assetTypeCode)}</td>
            <td>${safe(item.currencyCode)}</td>
            <td>${item.isActive ? t("yes") : t("no")}</td>
            <td class="table-actions">
                <button type="button" class="button button-small button-secondary" data-entity="asset" data-action="edit" data-id="${item.id}">${t("edit")}</button>
                <button type="button" class="button button-small button-danger" data-entity="asset" data-action="delete" data-id="${item.id}">${t("delete")}</button>
            </td>
        </tr>`);

    renderTable(el.transactionRows, state.transactions, 7, (item) => `
        <tr>
            <td>${dateTime(item.executedAt)}</td>
            <td>${translateTransactionType(item.type)}</td>
            <td>${safe(item.portfolioName)}</td>
            <td>${item.assetName ? safe(item.assetName) : "-"}</td>
            <td>${number(item.totalAmount)}</td>
            <td>${number(item.feeTotal)}</td>
            <td class="table-actions">
                <button type="button" class="button button-small button-secondary" data-entity="transaction" data-action="edit" data-id="${item.id}">${t("edit")}</button>
                <button type="button" class="button button-small button-danger" data-entity="transaction" data-action="delete" data-id="${item.id}">${t("delete")}</button>
            </td>
        </tr>`);

    renderTable(el.timelineRows, state.timeline, 2, (item) => `
        <tr><td>${safe(item.period)}</td><td>${number(item.netAmount)}</td></tr>`);

    renderTable(el.allocationRows, state.allocation, 7, (item) => `
        <tr>
            <td>${safe(item.assetName)}${item.assetSymbol ? ` <span class="muted">(${safe(item.assetSymbol)})</span>` : ""}</td>
            <td>${safe(item.portfolioName)}</td>
            <td>${number(item.quantity)}</td>
            <td>${number(item.costBasisAmount)}</td>
            <td>${item.latestPrice == null ? "-" : number(item.latestPrice)}</td>
            <td>${number(item.marketValue)}</td>
            <td class="${item.unrealizedProfit >= 0 ? "positive" : "negative"}">${number(item.unrealizedProfit)}</td>
        </tr>`);
}

function renderDashboardExperience() {
    renderDashboardHighlights();
    renderTimelineChart();
    renderAllocationChart();
    renderRecentActivity();
}

function renderDashboardHighlights() {
    const latest = state.timeline.at(-1) || null;
    const topPosition = [...state.allocation].sort((a, b) => Number(b.marketValue || 0) - Number(a.marketValue || 0))[0] || null;
    const strongestMover = [...state.allocation].sort((a, b) => Number(b.unrealizedProfit || 0) - Number(a.unrealizedProfit || 0))[0] || null;

    if (el.dashboardLatestPeriod) el.dashboardLatestPeriod.textContent = latest?.period || "-";
    if (el.dashboardLatestPeriodValue) el.dashboardLatestPeriodValue.textContent = latest ? number(latest.netAmount) : "0";

    if (el.dashboardTopPosition) {
        el.dashboardTopPosition.textContent = topPosition
            ? `${topPosition.assetName}${topPosition.assetSymbol ? ` (${topPosition.assetSymbol})` : ""}`
            : "-";
    }
    if (el.dashboardTopPositionValue) el.dashboardTopPositionValue.textContent = topPosition ? number(topPosition.marketValue) : "0";

    if (el.dashboardStrongestMover) {
        el.dashboardStrongestMover.textContent = strongestMover
            ? `${strongestMover.assetName}${strongestMover.assetSymbol ? ` (${strongestMover.assetSymbol})` : ""}`
            : "-";
    }
    if (el.dashboardStrongestMoverValue) {
        el.dashboardStrongestMoverValue.textContent = strongestMover ? number(strongestMover.unrealizedProfit) : "0";
        el.dashboardStrongestMoverValue.className = strongestMover
            ? `muted ${Number(strongestMover.unrealizedProfit || 0) >= 0 ? "positive" : "negative"}`
            : "muted";
    }
}

function renderTimelineChart() {
    if (!el.timelineChart) {
        return;
    }

    if (!state.timeline.length) {
        el.timelineChart.innerHTML = `<div class="chart-empty">${safe(t("noDataYet"))}</div>`;
        return;
    }

    const values = state.timeline.map((item) => Number(item.netAmount || 0));
    const max = Math.max(...values);
    const min = Math.min(...values);
    const range = max - min || 1;
    const width = 760;
    const height = 240;
    const paddingX = 20;
    const paddingY = 18;
    const stepX = state.timeline.length === 1 ? 0 : (width - paddingX * 2) / (state.timeline.length - 1);

    const points = state.timeline.map((item, index) => {
        const x = paddingX + stepX * index;
        const y = height - paddingY - ((Number(item.netAmount || 0) - min) / range) * (height - paddingY * 2);
        return { x, y, label: item.period, value: Number(item.netAmount || 0) };
    });

    const path = points.map((point, index) => `${index === 0 ? "M" : "L"} ${point.x} ${point.y}`).join(" ");
    const areaPath = `${path} L ${points.at(-1).x} ${height - paddingY} L ${points[0].x} ${height - paddingY} Z`;
    const labels = state.timeline.map((item) => `<span>${safe(item.period)}</span>`).join("");

    el.timelineChart.innerHTML = `
        <svg class="chart-svg" viewBox="0 0 ${width} ${height}" role="img" aria-label="${safe(t("timeline"))}">
            <defs>
                <linearGradient id="timelineAreaGradient" x1="0" y1="0" x2="0" y2="1">
                    <stop offset="0%" stop-color="#b85c2c" stop-opacity="0.24"></stop>
                    <stop offset="100%" stop-color="#b85c2c" stop-opacity="0.02"></stop>
                </linearGradient>
            </defs>
            <line class="chart-grid-line" x1="${paddingX}" y1="${height - paddingY}" x2="${width - paddingX}" y2="${height - paddingY}"></line>
            <line class="chart-grid-line" x1="${paddingX}" y1="${paddingY}" x2="${width - paddingX}" y2="${paddingY}"></line>
            <path class="chart-area" d="${areaPath}"></path>
            <path class="chart-line" d="${path}"></path>
            ${points.map((point) => `<circle class="chart-point" cx="${point.x}" cy="${point.y}" r="5"></circle>`).join("")}
        </svg>
        <div class="chart-label-row">${labels}</div>`;
}

function renderAllocationChart() {
    if (!el.allocationChart) {
        return;
    }

    if (!state.allocation.length) {
        el.allocationChart.innerHTML = `<div class="chart-empty">${safe(t("noDataYet"))}</div>`;
        return;
    }

    const topItems = [...state.allocation]
        .sort((a, b) => Number(b.marketValue || 0) - Number(a.marketValue || 0))
        .slice(0, 6);
    const maxValue = Math.max(...topItems.map((item) => Number(item.marketValue || 0)), 1);

    el.allocationChart.innerHTML = topItems.map((item) => {
        const width = Math.max(6, (Number(item.marketValue || 0) / maxValue) * 100);
        return `
            <div class="allocation-bar-row">
                <div class="allocation-bar-head">
                    <strong>${safe(item.assetName)}${item.assetSymbol ? ` <span class="muted">(${safe(item.assetSymbol)})</span>` : ""}</strong>
                    <span>${number(item.marketValue)}</span>
                </div>
                <div class="allocation-bar-track">
                    <div class="allocation-bar-fill" style="width:${width}%"></div>
                </div>
            </div>`;
    }).join("");
}

function renderRecentActivity() {
    if (!el.dashboardActivity) {
        return;
    }

    if (!state.transactions.length) {
        el.dashboardActivity.innerHTML = `<div class="chart-empty">${safe(t("noDataYet"))}</div>`;
        return;
    }

    const recentItems = [...state.transactions]
        .sort((a, b) => new Date(b.executedAt) - new Date(a.executedAt))
        .slice(0, 5);

    el.dashboardActivity.innerHTML = recentItems.map((item) => `
        <article class="activity-item">
            <div class="activity-meta">
                <span>${translateTransactionType(item.type)}</span>
                <span>${dateTime(item.executedAt)}</span>
            </div>
            <strong>${safe(item.portfolioName)}</strong>
            <span>${item.assetName ? safe(item.assetName) : "-"}</span>
            <strong class="${Number(item.totalAmount || 0) >= 0 ? "positive" : "negative"}">${number(item.totalAmount)}</strong>
        </article>`).join("");
}

async function savePortfolio(event) {
    event.preventDefault();

    try {
        const body = {
            name: el.portfolioName.value.trim(),
            baseCurrencyId: el.portfolioBaseCurrency.value
        };

        if (state.editing.portfolioId) {
            await api(`/api/v1/Portfolios/${state.editing.portfolioId}`, "PUT", {
                ...body,
                isArchived: el.portfolioIsArchived.checked
            });
            message(t("portfolioUpdated"), false, true);
        } else {
            await api("/api/v1/Portfolios", "POST", body);
            message(t("portfolioCreated"), false, true);
        }

        resetPortfolioForm();
        await hydrate();
    } catch (error) {
        message(error.message, true);
    }
}

async function saveAsset(event) {
    event.preventDefault();

    try {
        if (state.editing.assetId) {
            await api(`/api/v1/Assets/${state.editing.assetId}`, "PUT", {
                name: el.assetName.value.trim(),
                symbol: nullableString(el.assetSymbol.value),
                assetTypeId: el.assetType.value,
                currencyId: el.assetCurrency.value,
                exchangeId: nullableString(el.assetExchange.value),
                marketDataProviderId: nullableString(el.assetProvider.value),
                isActive: el.assetIsActive.checked
            });
            message(t("assetUpdated"), false, true);
        } else {
            await api("/api/v1/Assets", "POST", {
                portfolioId: el.assetPortfolio.value,
                name: el.assetName.value.trim(),
                symbol: nullableString(el.assetSymbol.value),
                assetTypeId: el.assetType.value,
                currencyId: el.assetCurrency.value,
                exchangeId: nullableString(el.assetExchange.value),
                marketDataProviderId: nullableString(el.assetProvider.value)
            });
            message(t("assetCreated"), false, true);
        }

        resetAssetForm();
        await hydrate();
    } catch (error) {
        message(error.message, true);
    }
}

async function saveTransaction(event) {
    event.preventDefault();

    try {
        const feeType = el.transactionFeeType.value.trim();
        const feeAmount = Number.parseFloat(el.transactionFeeAmount.value || "0");
        const body = {
            portfolioId: el.transactionPortfolio.value,
            assetId: nullableString(el.transactionAsset.value),
            type: Number.parseInt(el.transactionType.value, 10),
            executedAt: new Date(el.transactionExecutedAt.value).toISOString(),
            quantity: Number.parseFloat(el.transactionQuantity.value || "0"),
            unitPrice: Number.parseFloat(el.transactionUnitPrice.value || "0"),
            totalAmount: Number.parseFloat(el.transactionTotalAmount.value || "0"),
            description: nullableString(el.transactionDescription.value),
            fees: feeType && feeAmount !== 0 ? [{ feeType, amount: feeAmount }] : []
        };

        if (state.editing.transactionId) {
            await api(`/api/v1/Transactions/${state.editing.transactionId}`, "PUT", body);
            message(t("transactionUpdated"), false, true);
        } else {
            await api("/api/v1/Transactions", "POST", body);
            message(t("transactionCreated"), false, true);
        }

        resetTransactionForm();
        await hydrate();
    } catch (error) {
        message(error.message, true);
    }
}

async function handlePortfolioRowAction(event) {
    const button = event.target.closest("button[data-entity='portfolio']");
    if (!button) return;

    const portfolio = state.portfolios.find((item) => item.id === button.dataset.id);
    if (!portfolio) return;

    if (button.dataset.action === "edit") {
        state.editing.portfolioId = portfolio.id;
        el.portfolioName.value = portfolio.name;
        el.portfolioBaseCurrency.value = portfolio.baseCurrencyId;
        el.portfolioIsArchived.checked = portfolio.isArchived;
        renderFormLabels();
        return;
    }

    if (!confirm(format(t("confirmDeletePortfolio"), { name: portfolio.name }))) return;

    try {
        await api(`/api/v1/Portfolios/${portfolio.id}`, "DELETE");
        if (state.editing.portfolioId === portfolio.id) resetPortfolioForm();
        await hydrate();
        message(t("portfolioDeleted"), false, true);
    } catch (error) {
        message(error.message, true);
    }
}

async function handleAssetRowAction(event) {
    const button = event.target.closest("button[data-entity='asset']");
    if (!button) return;

    const asset = state.assets.find((item) => item.id === button.dataset.id);
    if (!asset) return;

    if (button.dataset.action === "edit") {
        state.editing.assetId = asset.id;
        el.assetPortfolio.value = asset.portfolioId;
        el.assetPortfolio.disabled = true;
        el.assetName.value = asset.name;
        el.assetSymbol.value = asset.symbol || "";
        el.assetType.value = asset.assetTypeId;
        el.assetCurrency.value = asset.currencyId;
        el.assetExchange.value = asset.exchangeId || "";
        el.assetProvider.value = asset.marketDataProviderId || "";
        el.assetIsActive.checked = asset.isActive;
        renderFormLabels();
        return;
    }

    if (!confirm(format(t("confirmDeleteAsset"), { name: asset.name }))) return;

    try {
        await api(`/api/v1/Assets/${asset.id}`, "DELETE");
        if (state.editing.assetId === asset.id) resetAssetForm();
        await hydrate();
        message(t("assetDeleted"), false, true);
    } catch (error) {
        message(error.message, true);
    }
}

async function handleTransactionRowAction(event) {
    const button = event.target.closest("button[data-entity='transaction']");
    if (!button) return;

    const transaction = state.transactions.find((item) => item.id === button.dataset.id);
    if (!transaction) return;

    if (button.dataset.action === "edit") {
        state.editing.transactionId = transaction.id;
        el.transactionPortfolio.value = transaction.portfolioId;
        renderTransactionAssetOptions();
        el.transactionAsset.value = transaction.assetId || "";
        el.transactionType.value = transactionTypeToValue(transaction.type);
        el.transactionExecutedAt.value = toLocalDateTimeValue(new Date(transaction.executedAt));
        el.transactionQuantity.value = transaction.quantity;
        el.transactionUnitPrice.value = transaction.unitPrice;
        el.transactionTotalAmount.value = transaction.totalAmount;
        el.transactionDescription.value = transaction.description || "";
        el.transactionFeeType.value = transaction.fees?.[0]?.feeType || "";
        el.transactionFeeAmount.value = transaction.fees?.[0]?.amount || 0;
        renderFormLabels();
        return;
    }

    if (!confirm(t("confirmDeleteTransaction"))) return;

    try {
        await api(`/api/v1/Transactions/${transaction.id}`, "DELETE");
        if (state.editing.transactionId === transaction.id) resetTransactionForm();
        await hydrate();
        message(t("transactionDeleted"), false, true);
    } catch (error) {
        message(error.message, true);
    }
}

function resetPortfolioForm() {
    state.editing.portfolioId = null;
    el.portfolioForm?.reset();
    if (el.portfolioIsArchived) el.portfolioIsArchived.checked = false;
    renderFormLabels();
}

function resetAssetForm() {
    state.editing.assetId = null;
    el.assetForm?.reset();
    if (el.assetPortfolio) el.assetPortfolio.disabled = false;
    if (el.assetIsActive) el.assetIsActive.checked = true;
    renderFormLabels();
}

function resetTransactionForm() {
    state.editing.transactionId = null;
    el.transactionForm?.reset();
    if (el.transactionExecutedAt) el.transactionExecutedAt.value = toLocalDateTimeValue(new Date());
    if (el.transactionFeeAmount) el.transactionFeeAmount.value = 0;
    renderFormLabels();
}

function renderFormLabels() {
    if (el.portfolioFormTitle) {
        el.portfolioFormTitle.textContent = state.editing.portfolioId ? t("updatePortfolio") : t("createPortfolio");
        el.portfolioSubmit.textContent = state.editing.portfolioId ? t("updatePortfolio") : t("createPortfolio");
        el.portfolioCancel.hidden = !state.editing.portfolioId;
    }

    if (el.assetFormTitle) {
        el.assetFormTitle.textContent = state.editing.assetId ? t("updateAsset") : t("createAsset");
        el.assetSubmit.textContent = state.editing.assetId ? t("updateAsset") : t("createAsset");
        el.assetCancel.hidden = !state.editing.assetId;
    }

    if (el.transactionFormTitle) {
        el.transactionFormTitle.textContent = state.editing.transactionId ? t("updateTransaction") : t("createTransaction");
        el.transactionSubmit.textContent = state.editing.transactionId ? t("updateTransaction") : t("createTransaction");
        el.transactionCancel.hidden = !state.editing.transactionId;
    }
}

function clearTables() {
    [el.portfolioRows, el.assetRows, el.transactionRows, el.timelineRows, el.allocationRows].forEach((target) => {
        if (target) target.innerHTML = "";
    });
    renderSummary();
    renderTables();
}

function renderTable(target, items, colspan, rowTemplate) {
    if (!target) {
        return;
    }

    if (!items.length) {
        target.innerHTML = `<tr><td colspan="${colspan}" class="muted">${safe(t("noDataYet"))}</td></tr>`;
        return;
    }

    target.innerHTML = items.map(rowTemplate).join("");
}

function setOptions(select, items, options = {}) {
    if (!select) {
        return;
    }

    const allowEmpty = options.allowEmpty ?? false;
    const selectedValue = options.selectedValue ?? select.value;
    const rows = [`<option value="" ${allowEmpty ? "" : "disabled"}>${safe(options.emptyLabel || "")}</option>`];
    items.forEach((item) => rows.push(`<option value="${item.value}">${safe(item.label)}</option>`));
    select.innerHTML = rows.join("");

    if (selectedValue) {
        select.value = selectedValue;
    } else if (!allowEmpty && items.length) {
        select.value = String(items[0].value);
    }
}

async function api(path, method = "GET", body, allowRefresh = true) {
    const headers = new Headers();
    if (body !== undefined) headers.set("Content-Type", "application/json");
    if (state.jwt) headers.set("Authorization", `Bearer ${state.jwt}`);

    let response;
    try {
        response = await fetch(`${normalizeBaseUrl(state.apiBaseUrl)}${path}`, {
            method,
            headers,
            body: body === undefined ? undefined : JSON.stringify(body)
        });
    } catch {
        throw new Error(t("connectionIssue"));
    }

    if (response.status === 401 && allowRefresh && state.refreshToken && !path.includes("RefreshTokenData")) {
        try {
            await refreshTokens();
            return api(path, method, body, false);
        } catch (error) {
            handleSessionFailure(error);
            throw error;
        }
    }

    if (!response.ok) {
        const payload = await readJson(response);
        if (typeof payload === "string" && payload.trim().startsWith("<!DOCTYPE html")) {
            throw new Error(t("apiReturnedHtml"));
        }
        throw new Error(payload?.error || payload?.title || payload || response.statusText || "Request failed.");
    }

    if (response.status === 204) {
        return null;
    }

    return readJson(response);
}

async function refreshTokens() {
    if (!state.jwt || !state.refreshToken) {
        throw new Error("JWT or refresh token is missing.");
    }

    const response = await api("/api/v1/identity/account/RefreshTokenData", "POST", {
        jwt: state.jwt,
        refreshToken: state.refreshToken
    }, false);

    setSession(response.jwt, response.refreshToken);
}

async function readJson(response) {
    const contentType = response.headers.get("content-type") || "";
    return contentType.includes("application/json") ? response.json() : response.text();
}

function setSession(jwt, refreshToken) {
    state.jwt = jwt;
    state.refreshToken = refreshToken;
    state.isAuthenticated = true;
    localStorage.setItem(keys.jwt, jwt);
    localStorage.setItem(keys.refreshToken, refreshToken);
    syncSession();
}

function clearSession() {
    state.jwt = null;
    state.refreshToken = null;
    state.isAuthenticated = false;
    localStorage.removeItem(keys.jwt);
    localStorage.removeItem(keys.refreshToken);
    syncSession();
}

function normalizeSessionErrorMessage(error) {
    const text = String(error?.message || "").trim();
    if (!text) {
        return t("signInFirst");
    }

    if (/refreshtokens?/i.test(text) || /refresh token/i.test(text) || /401/.test(text)) {
        return t("sessionExpired");
    }

    return text;
}

function handleSessionFailure(error, options = {}) {
    const text = normalizeSessionErrorMessage(error);
    clearSession();
    setPage("dashboard", false);
    if (!options.silent) {
        message(text, true);
    }
}

function syncSession() {
    if (!el.authStatus) {
        return;
    }

    const hasSession = Boolean(state.isAuthenticated && state.jwt);
    const authStatus = hasSession ? t("signedIn") : t("signedOut");
    el.authStatus.textContent = authStatus;
    el.jwtState.textContent = hasSession ? t("present") : t("missing");
    el.refreshState.textContent = hasSession && state.refreshToken ? t("present") : t("missing");
    if (el.topbarAuthStatus) el.topbarAuthStatus.textContent = authStatus;
    if (el.accessAuthStatus) el.accessAuthStatus.textContent = authStatus;
    if (el.accessJwtStatus) el.accessJwtStatus.textContent = hasSession ? t("present") : t("missing");
    if (el.accessRefreshStatus) el.accessRefreshStatus.textContent = hasSession && state.refreshToken ? t("present") : t("missing");
    if (el.dashboardAuthStatus) el.dashboardAuthStatus.textContent = authStatus;
    if (el.authOverlay) el.authOverlay.classList.toggle("is-hidden", hasSession);
    if (el.appShell) el.appShell.classList.toggle("is-authenticated", hasSession);
    if (el.authOverlay) el.authOverlay.hidden = hasSession;
    if (el.appShell) el.appShell.hidden = !hasSession;
    document.body.dataset.session = hasSession ? "auth" : "guest";
}

function applyTransactionTypeLabels() {
    if (!el.transactionType) {
        return;
    }

    const labels = {
        1: t("buy"),
        2: t("sell"),
        3: t("deposit"),
        4: t("withdrawal"),
        5: t("dividend")
    };

    Array.from(el.transactionType.options).forEach((option) => {
        option.textContent = labels[option.value] || option.textContent;
    });
}

function message(text, isError = false, isSuccess = false) {
    el.message.hidden = false;
    el.message.textContent = text;
    el.message.className = "message";
    if (isError) el.message.classList.add("is-error");
    if (isSuccess) el.message.classList.add("is-success");
}

function t(key) {
    return translations[state.language]?.[key] ?? translations.en[key] ?? fallbackText[key] ?? key;
}

function format(template, values = {}) {
    return Object.entries(values).reduce((current, [key, value]) => current.replaceAll(`{${key}}`, String(value)), template);
}

function mapLookup(item) {
    return { value: item.id, label: item.displayName || item.code };
}

function translateTransactionType(type) {
    return t(String(type || "").toLowerCase()) || type || "";
}

function transactionTypeToValue(type) {
    const map = { Buy: "1", Sell: "2", Deposit: "3", Withdrawal: "4", Dividend: "5" };
    return map[type] || String(type || "1");
}

function nullableString(value) {
    const trimmed = value.trim();
    return trimmed.length ? trimmed : null;
}

function normalizeBaseUrl(value) {
    return String(value || "").trim().replace(/\/+$/, "");
}

function formatApiBaseLabel(value) {
    try {
        const url = new URL(normalizeBaseUrl(value));
        return url.host;
    } catch {
        return normalizeBaseUrl(value) || "-";
    }
}

function normalizeLanguage(value) {
    return String(value || "").toLowerCase().startsWith("et") ? "et" : "en";
}

function number(value) {
    return new Intl.NumberFormat(state.language === "et" ? "et-EE" : "en-US", {
        maximumFractionDigits: 2
    }).format(Number(value || 0));
}

function dateTime(value) {
    return new Date(value).toLocaleString(state.language === "et" ? "et-EE" : "en-US");
}

function toLocalDateTimeValue(date) {
    const shifted = new Date(date.getTime() - date.getTimezoneOffset() * 60000);
    return shifted.toISOString().slice(0, 16);
}

function safe(value) {
    return String(value ?? "")
        .replaceAll("&", "&amp;")
        .replaceAll("<", "&lt;")
        .replaceAll(">", "&gt;")
        .replaceAll('"', "&quot;")
        .replaceAll("'", "&#39;");
}
