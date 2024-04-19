using Daily.API.ApiReponses;
using Daily.API.DataModel;
using Daily.API.Dtos;
using Daily.API.Helper;
using Daily.API.IServices;
using Daily.API.Repositories;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.Design;

namespace Daily.API.Services;

/// <summary>
/// 实现备忘录相关的服务类，如添加、编辑、删除备忘录等
/// </summary>
public class MemoService : IMemoService
{
    /// <summary>
    /// 备忘录仓储层服务
    /// </summary>
    private readonly IMemoRepoService _memoRepoService;

    /// <summary>
    /// 响应帮助类，用于生成响应信息
    /// </summary>
    private readonly IApiResponseHelper _apiResponseHelper;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="apiResponseHelper">响应帮助类</param>
    /// <param name="memoRepoService">备忘录仓储层服务</param>
    public MemoService(IApiResponseHelper apiResponseHelper, IMemoRepoService memoRepoService)
    {
        _memoRepoService = memoRepoService;
        _apiResponseHelper = apiResponseHelper;
    }

    /// <summary>
    /// 实现添加备忘录的服务
    /// </summary>
    /// <param name="memoInfoDto">备忘录Dto</param>
    /// <returns>返回一个ApiResponse类型的对象，包括添加是否成功和异常信息等</returns>
    public async Task<ApiResponse> AddMemoAsync(MemoInfoDto memoInfoDto)
    {
        ApiResponse apiResponse = new();

        ArgumentNullException.ThrowIfNull(memoInfoDto);

        try
        {
            if(string.IsNullOrEmpty(memoInfoDto.Content) || string.IsNullOrEmpty(memoInfoDto.Title))
            {
                apiResponse.ResultCode = ResultCodeEnum.DtoNotComplete;
                apiResponse.ResultData = new object();
                apiResponse.Msg = "备忘录信息不全";
            }

            var model = new MemoInfo() 
            {
                Title = memoInfoDto.Title,
                Content = memoInfoDto.Content,
                Status = memoInfoDto.Status
            };

            var result = await _memoRepoService.AddMemoAsync(model);

            if(result.Success && result.Code == 0)
            {
                apiResponse.Msg = "添加备忘录成功";
                apiResponse.ResultCode = ResultCodeEnum.Success;
                apiResponse.ResultData = result.Code;
            }

            if(!result.Success && result.Code == 5)
            {
                apiResponse.Msg = "备忘录信息不全";
                apiResponse.ResultCode = ResultCodeEnum.DtoNotComplete;
                apiResponse.ResultData = result.Code;
            }

            if(!result.Success && result.Code == 10)
            {
                apiResponse.Msg = "服务器忙，请稍后...";
                apiResponse.ResultCode = ResultCodeEnum.DataBaseError;
                apiResponse.ResultData = result.Code;
            }   
            return apiResponse;
        }
        catch (Exception ex)
        {
            apiResponse = _apiResponseHelper.GenerateErrorResponse(ex);
        }

        return apiResponse;
    }

    /// <summary>
    /// 实现删除备忘录的服务
    /// </summary>
    /// <param name="id">备忘录id</param>
    /// <returns>返回也给ApiResponse类型的对象，包括删除成功与否和其他信息等</returns>
    public async Task<ApiResponse> DeleteMemoAsync(int id)
    {
        ApiResponse apiResponse = new();

        try
        {
            if(id <= 0)
            {
                apiResponse.Msg = "服务器忙，请稍后...";
                apiResponse.ResultData = new object();
                apiResponse.ResultCode = ResultCodeEnum.DtoError;
                return apiResponse;
            }

            var result = await _memoRepoService.DeleteMemoAsync(id);

            if(result.Success && result.Code == 0)
            {
                apiResponse.Msg = "删除备忘录成功";
                apiResponse.ResultCode = ResultCodeEnum.Success;
                apiResponse.ResultData = result.Code;
            }

            if(!result.Success && result.Code == 5)
            {
                apiResponse.Msg = "服务器忙，请稍后...";
                apiResponse.ResultCode = ResultCodeEnum.DtoError;
                apiResponse.ResultData = result.Code;
            }

            if(!result.Success && result.Code == 10)
            {
                apiResponse.Msg = "找不到该备忘录";
                apiResponse.ResultCode = ResultCodeEnum.DataBaseError;
                apiResponse.ResultData = result.Code;
            }

            return apiResponse;
        }
        catch (Exception ex)
        {
            apiResponse = _apiResponseHelper.GenerateErrorResponse(ex);
        }
        return apiResponse;
    }

    /// <summary>
    /// 实现编辑备忘录的服务
    /// </summary>
    /// <param name="editMemoDto">编辑备忘录Dto</param>
    /// <returns>返回也给ApiResponse类型的对象，包括编辑成功与否和其他信息</returns>
    public async Task<ApiResponse> EditMemoAsync(EditMemoDto editMemoDto)
    {
        ApiResponse apiResponse = new();

        ArgumentNullException.ThrowIfNull(editMemoDto);

        if(string.IsNullOrEmpty(editMemoDto.Content) || string.IsNullOrEmpty(editMemoDto.Title))
        {
            apiResponse.ResultCode = ResultCodeEnum.DtoNotComplete;
            apiResponse.ResultData = new object();
            apiResponse.Msg = "备忘录信息不全";
        }

        try
        {
            var model = new MemoInfo()
            {
                MemoId = editMemoDto.MemoId,
                Title = editMemoDto.Title,
                Content = editMemoDto.Content,
                Status = editMemoDto.Status
            };

            var result = await _memoRepoService.UpdateMemoAsync(model);

            if( result== 0)
            {
                apiResponse.Msg = "编辑备忘录成功";
                apiResponse.ResultCode = ResultCodeEnum.Success;
                apiResponse.ResultData = result;
            }

            if(result == 5)
            {
                apiResponse.Msg = "备忘录信息不全";
                apiResponse.ResultCode = ResultCodeEnum.DtoNotComplete;
                apiResponse.ResultData = result;
            }

            if(result == 10)
            {
                apiResponse.Msg = "找不到备忘录";
                apiResponse.ResultCode = ResultCodeEnum.DataBaseError;
                apiResponse.ResultData = result;
            }

            if(result == 15)
            {
                apiResponse.Msg = "服务器忙，请稍后...";
                apiResponse.ResultCode = ResultCodeEnum.DataBaseError;
                apiResponse.ResultData = result;
            }

            return apiResponse;
        }
        catch (Exception ex)
        {
            apiResponse = _apiResponseHelper.GenerateErrorResponse(ex);
        }
        return apiResponse;
    }

    /// <summary>
    /// 实现获取所有备忘录的服务
    /// </summary>
    /// <returns></returns>
    public async Task<ApiResponse> GetAllMemoListAsync()
    {
        ApiResponse apiResponse = new();

        try
        {
            var queryResult = await _memoRepoService.GetAllMemosAsync();

            if(queryResult.Code == 0)
            {
                apiResponse.Msg = "获取所有备忘录成功";
                apiResponse.ResultData = queryResult.memoInfoList;
                apiResponse.ResultCode = ResultCodeEnum.Success;
            }
            return apiResponse;
        }
        catch (Exception ex)
        {
            apiResponse = _apiResponseHelper.GenerateErrorResponse(ex);
        }
        return apiResponse;
    }

    /// <summary>
    /// 实现根据文本条件查询备忘录的服务
    /// </summary>
    /// <param name="searchText">查询文本</param>
    /// <returns>返回一个ApiResponse类型的对象，包括满足条件的备忘录和其他信息</returns>
    public async Task<ApiResponse> GetConditionQueryMemoListAsync(string searchText)
    {
        ApiResponse apiResponse = new();
        try
        {
            var quertyResult = await _memoRepoService.SearchMemosAsync(searchText);

            if(quertyResult.Code == 0)
            {
                apiResponse.Msg = "获取查询的备忘录成功";
                apiResponse.ResultData = quertyResult.memoInfoList;
                apiResponse.ResultCode = ResultCodeEnum.Success;
            }
            return apiResponse;
        }
        catch (Exception ex)
        {
            apiResponse = _apiResponseHelper.GenerateErrorResponse(ex);
        }
        return apiResponse;
    }
}
