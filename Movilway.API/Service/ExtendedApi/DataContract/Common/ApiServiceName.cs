﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Movilway.API.Service.ExtendedApi.DataContract.Common
{
    public enum ApiServiceName
    {
        AddTrustedDevice,
        BlockAgent,
        BuyStock,
        Cash,
        CashIn,
        CashOut,
        ChangePin,
        ChangeBranchStatus,
        ChangePassword,
        ConsolidateMoviPin,
        CreateAgent,
        CreateMoviPin,
        CreditNote,
        DebitNote,
        DeleteTrustedDevice,
        ExternalTransfer,
        GetAgent,
        GetAgentInfo,
        GetAgentClosingDetails,
        GetAuditoryUsersReport,
        GetParameters,
        GetAgentGroups,
        GetBalance,
        GetLoanToken,
        GetBankList,
        GetBankListApp,
        GetChildList,
        GetChildListById,
        GetAgentDistributionList,
        GetUserDistributionList,
        GetCityList,
        GetCutsProvider,
        GetExternalBalance,
        GetGroupList,
        GetLatestAccess,
        GetLastTransactions,
        GetLastDistributions,
        GetDistributionsMadeByAgent,
        GetParentList,
        GetPendingDistributions,
        GetPinUsers,
        GetProductList,
        GetMacroProductList,
        GetProvinceList,
        GetPurchasesSummaryDashBoard,
        GetRoles,
        GetSession,
        GetTodayMoviPayments,
        GetTransaction,
        GetTransactionByExternalReference,
        GetTransactionsInRange,
        GetTransactionsReport,
        GetTrustedDevices,
        GetTypeAccess,
        GetSalesSummary,
        GetSalesSummaryReport,
        GetSalesSummaryDashBoard,
        GetScore,
        GetUsersAgent,
        InformPayment,
        LoginAvailable,
        MapAgentToGroup,
        MonthlyReport,
        MoviPayment,
        Notiway,
        Payment,
        PayStock,
        ProcessExternalTransaction,
        ProcessDistribution,
        QueryPayment,
        RegenerateMoviPin,
        RegisterDebtPayment,
        RegisterAgent,
        RegisterAgentBulk,
        RequestStock,
        Security,
        Sell,
        SetStateTrustedDevice,
        SetUserStatus,
        SOSIT,
        Transfer,
        TransferCommission,
        TransferStock,
        TopUp,
        UpdateAgent,
        UnBlockAgent,
        UnMapAgentToGroup,
        UpdateContactInfoAgent,
        ValidateDeposit,
        ValidateDevice,
        ValidateMoviPin,
        SetAgentClose,
        GetAgentDistributions,
        GetAgentClosings,
        GetNewAgentsReport,
        GetDailyCutReport,
        GetCutStock,
        GetBlockedAgenciesByInactivityReport
    }
}