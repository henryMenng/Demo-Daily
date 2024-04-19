using Daily.API.ApiReponses;
using Daily.API.DataModel;
using Daily.API.Dtos;
using Daily.API.Helper;
using Daily.API.IServices;
using Daily.API.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Daily.API.Services;

/// <summary>
/// 实现备忘录相关接口的服务类，包括添加、编辑、删除、查询等
/// </summary>
public class ToDoService : IToDoService
{

    private readonly IToDoInfoRepoService _toDoInfoRepoService;

    /// <summary>
    /// 响应帮助类
    /// </summary>
    private readonly IApiResponseHelper _apiResponseHelper;

    /// <summary>
    /// 待办事项服务类的构造函数
    /// </summary>
    /// <param name="apiResponseHelper">响应帮助类</param>
    /// <param name="toDoInfoRepoService">待办实现仓储服务</param>
    public ToDoService(IApiResponseHelper apiResponseHelper, IToDoInfoRepoService toDoInfoRepoService)
    {
        _toDoInfoRepoService = toDoInfoRepoService;
        _apiResponseHelper = apiResponseHelper;
    }

    /// <summary>
    /// 添加待办事项
    /// </summary>
    /// <param name="addToDoDto">添加待办Dto</param>
    /// <returns>返回一个ApiResponse，包括添加是否成功或异常信息</returns>
    public async Task<ApiResponse> AddToDoAsync(AddToDoDto addToDoDto)
    {
        // 创建一个ApiResponse对象，用于返回添加操作的结果信息
        ApiResponse apiResponse = new();

        ArgumentNullException.ThrowIfNull(addToDoDto);

        try
        {

            // 创建一个ToDoInfo对象，用于添加到数据库
            var addToDo = new ToDoInfo() 
            {
                Title = addToDoDto.Title,
                Content = addToDoDto.Content,
                Status = addToDoDto.Status
            };
            var result = await _toDoInfoRepoService.AddToDoInfoAsync(addToDo);

            if(result.Success)
            {
                apiResponse.ResultCode = ResultCodeEnum.Success;
                apiResponse.Msg = "添加待办事项成功";
                apiResponse.ResultData = result.Code;
                return apiResponse;
            }
            else
            {
                apiResponse.ResultCode = ResultCodeEnum.Error;
                apiResponse.Msg = "添加待办事项失败";
                apiResponse.ResultData = result.Code;
                return apiResponse;
            }
        }
        catch (Exception ex)
        {
            // 如果发生异常，则返回异常错误,通过响应帮助类生成异常响应信息
            apiResponse = _apiResponseHelper.GenerateErrorResponse(ex);
        }
        // 返回添加操作的结果信息
        return _apiResponseHelper.GenerateErrorResponse();
    }

    /// <summary>
    /// 删除待办事项
    /// </summary>
    /// <param name="id">待办事项id，查询字符串</param>
    /// <returns>返回一个ApiResponse，包括删除是否成功或异常信息</returns>
    public async Task<ApiResponse> DeleteToDoAsync(int id)
    {
        // 创建一个ApiResponse对象，用于返回删除操作的结果信息
        ApiResponse apiResponse = new();

        try
        {
            var result = await _toDoInfoRepoService.DeleteToDoInfoAsync(id);

            if(result.Success && result.Code == 0)
            {
               apiResponse.ResultCode = ResultCodeEnum.Success;
                apiResponse.Msg = "删除成功";
                apiResponse.ResultData = result.Code;
                return apiResponse;
            }
            if (!result.Success && result.Code == 5)
            {
                apiResponse.ResultCode = ResultCodeEnum.DtoError;
                apiResponse.Msg = "服务器忙，请稍后...";
                apiResponse.ResultData = result.Code;
                return apiResponse;
            }
            if(!result.Success && result.Code == 10)
            {
                apiResponse.ResultCode = ResultCodeEnum.NotFound;
                apiResponse.Msg = "待办事项不存在";
                apiResponse.ResultData = result.Code;
                return apiResponse;
            }
        }
        catch (Exception ex)
        {
            // 如果发生异常，则返回异常错误,通过响应帮助类生成异常响应信息
            apiResponse = _apiResponseHelper.GenerateErrorResponse(ex);
        }
        // 返回删除操作的结果信息
        return _apiResponseHelper.GenerateErrorResponse();
    }

    /// <summary>
    /// 编辑待办事项
    /// </summary>
    /// <param name="editToDoDto">编辑待办事项Dto</param>
    /// <returns>返回一个ApiResponse，包括编辑是否成功或异常信息</returns>
    public async Task<ApiResponse> EditToDoAsync(EditToDoDto editToDoDto)
    {
        // 创建一个ApiResponse对象，用于返回编辑操作的结果信息
        ApiResponse apiResponse = new();

        ArgumentNullException.ThrowIfNull(editToDoDto);

        try
        {
            if(string.IsNullOrEmpty(editToDoDto.Content) || string.IsNullOrEmpty(editToDoDto.Title))
            {
                apiResponse.ResultCode = ResultCodeEnum.DtoNotComplete;
                apiResponse.Msg = "待办事项信息不全";
                apiResponse.ResultData = new object() ;
                return apiResponse;
            }

            var model = new ToDoInfo()
            {
                ToDoId = editToDoDto.ToDoId,
                Title = editToDoDto.Title,
                Content = editToDoDto.Content,
                Status = editToDoDto.Status
            };

            var result = await _toDoInfoRepoService.UpdateToDoInfoAsync(model);

            if(result.Success && result.Code == 0)
            {
                apiResponse.ResultCode = ResultCodeEnum.Success;
                apiResponse.Msg = "编辑成功";
                apiResponse.ResultData = result.Code;
                return apiResponse;
            }

            if (!result.Success && result.Code == 5)
            {
                apiResponse.ResultCode = ResultCodeEnum.DtoError;
                apiResponse.Msg = "服务器忙，请稍后...";
                apiResponse.ResultData = result.Code;
                return apiResponse;
            }

            if (!result.Success && result.Code == 10)
            {
                apiResponse.ResultCode = ResultCodeEnum.NotFound;
                apiResponse.Msg = "待办事项不存在";
                apiResponse.ResultData = result.Code;
                return apiResponse;
            }
        }
        catch (Exception ex)
        {
            // 如果发生异常，则返回异常错误,通过响应帮助类生成异常响应信息
            apiResponse = _apiResponseHelper.GenerateErrorResponse(ex);
        }
        // 返回编辑操作的结果信息
        return _apiResponseHelper.GenerateErrorResponse();
    }

    /// <summary>
    /// 获取全部待办事项
    /// </summary>
    /// <returns>返回一个ApiResponse，包括所有待办事项或异常信息</returns>
    public async Task<ApiResponse> GetAllToDoListAsync()
    {
        // 创建一个ApiResponse对象，用于返回获取操作的结果信息
        ApiResponse apiResponse = new ();

        try
        {
            var result = await _toDoInfoRepoService.GetConditionQueryToDoListAsync(0, "");

            if(result.Success && result.Code == 0)
            {
                apiResponse.ResultCode = ResultCodeEnum.Success;
                apiResponse.Msg = "获取全部待办事项成功！";
                apiResponse.ResultData = result.toDoInfoList.Select(temp => new ToDoInfoDto()
                {
                    Content = temp.Content,
                    Status = temp.Status,
                    Title = temp.Title,
                    ToDoId = temp.ToDoId
                }).ToList();
                return apiResponse;
            }
            else
            {
                apiResponse.ResultCode = ResultCodeEnum.Error;
                apiResponse.Msg = "获取全部待办事项失败！";
                apiResponse.ResultData = result.Code;
            }
        }
        catch (Exception ex)
        {
            // 如果发生异常，则返回异常错误,通过响应帮助类生成异常响应信息
            apiResponse = _apiResponseHelper.GenerateErrorResponse(ex);
        }
        // 返回获取操作的结果信息
        return _apiResponseHelper.GenerateErrorResponse();
    }

    /// <summary>
    /// 根据条件获得待办事项
    /// </summary>
    /// <param name="status">待办事项状态，查询字符串</param>
    /// <param name="searchText">条件文本，查询字符串</param>
    /// <returns>返回一个ApiResponse，包括查询的待办事项或异常信息</returns>
    public async Task<ApiResponse> GetConditionQueryToDoListAsync(int status, string? searchText)
    {
        // 创建一个ApiResponse对象，用于返回查询操作的结果信息
        ApiResponse apiResponse = new();

        try
        {
            var result = await _toDoInfoRepoService.GetConditionQueryToDoListAsync(status,searchText);
            if(!result.Success)
            {
                apiResponse.ResultCode = ResultCodeEnum.Error;
                apiResponse.Msg = "查询失败";
                apiResponse.ResultData = null;
            }

            status -= 1;
            string statusName = string.Empty;
            switch (status)
            {
                case -1:
                    statusName = "全部";
                    break;
                case 0:
                    statusName = "待办中";
                    break;
                case 1:
                    statusName = "已完成";
                    break;
                default:
                    break;
            }
            apiResponse.ResultCode = ResultCodeEnum.Success;
            if(string.IsNullOrEmpty(searchText))
                if(status != -1)
                    apiResponse.Msg = $"获取全部{statusName}事项成功！";
                else
                    apiResponse.Msg = $"获取全部待办事项成功！";
            else
                apiResponse.Msg = $"获取{statusName}且内容或标题有{searchText}的待办事项成功！";
                apiResponse.ResultData = result.toDoInfoList.Select(temp => new ToDoInfoDto()
                {
                    Content = temp.Content,
                    Status = temp.Status,
                    Title = temp.Title,
                    ToDoId = temp.ToDoId
                }).ToList();
                return apiResponse;
        }
        catch (Exception ex)
        {
            // 如果发生异常，则返回异常错误,通过响应帮助类生成异常响应信息
            apiResponse = _apiResponseHelper.GenerateErrorResponse(ex);
        }
        // 返回查询操作的结果信息
        return _apiResponseHelper.GenerateErrorResponse();
    }

    /// <summary>
    /// 获得待办中事项列表（未完成）
    /// </summary>
    /// <returns>返回一个ApiResponse，包括待办中的事项或异常信息</returns>
    public async Task<ApiResponse> GetActiveToDoListAsync()
    {
        // 创建一个ApiResponse对象，用于返回获取操作的结果信息
        ApiResponse apiResponse = new();

        try
        {
            var result = await _toDoInfoRepoService.GetConditionQueryToDoListAsync(1, "");

            if (result.Success && result.Code == 10)
            {
                apiResponse.ResultCode = ResultCodeEnum.Success;
                apiResponse.Msg = "获取待办中的事项成功！";
                apiResponse.ResultData = result.toDoInfoList.Select(temp => new ToDoInfoDto()
                {
                    Content = temp.Content,
                    Status = temp.Status,
                    Title = temp.Title,
                    ToDoId = temp.ToDoId
                }).ToList();
                return apiResponse;
            }
            else
            {
                apiResponse.ResultCode = ResultCodeEnum.Error;
                apiResponse.Msg = "获取待办中的事项失败！";
                apiResponse.ResultData = result.Code;
            }
        }
        catch (Exception ex)
        {
            // 如果发生异常，则返回异常错误,通过响应帮助类生成异常响应信息
            apiResponse = _apiResponseHelper.GenerateErrorResponse(ex);
        }
        // 返回获取操作的结果信息
        return _apiResponseHelper.GenerateErrorResponse();
    }

    /// <summary>
    /// 获得已完成事项列表
    /// </summary>
    /// <returns>返回一个ApiResponse，包括已完成的事项或异常信息</returns>
    public async Task<ApiResponse> GetCompletedToDoListAsync()
    {
        // 创建一个ApiResponse对象，用于返回获取操作的结果信息
        ApiResponse apiResponse = new();

        try
        {
            var result = await _toDoInfoRepoService.GetConditionQueryToDoListAsync(2, "");
            if(result.Success && result.Code == 20)
            {
                apiResponse.ResultCode = ResultCodeEnum.Success;
                apiResponse.Msg = "获取已完成的事项成功！";
                apiResponse.ResultData = result.toDoInfoList.Select(temp => new ToDoInfoDto()
                {
                    Content = temp.Content,
                    Status = temp.Status,
                    Title = temp.Title,
                    ToDoId = temp.ToDoId
                }).ToList();
                return apiResponse;
            }
            else
            {
                apiResponse.ResultCode = ResultCodeEnum.Error;
                apiResponse.Msg = "获取已完成的事项失败！";
                apiResponse.ResultData = result.Code;
            }
        }
        catch (Exception ex)
        {
            // 如果发生异常，则返回异常错误,通过响应帮助类生成异常响应信息
            apiResponse = _apiResponseHelper.GenerateErrorResponse(ex);
        }
        // 返回获取操作的结果信息
        return _apiResponseHelper.GenerateErrorResponse();
    }

    /// <summary>
    /// 统计待办事项，包括总数和已完成数
    /// </summary>
    /// <returns>返回一个ApiResponse，包括总数已完成数和完成百分比或异常信息</returns>
    public async Task<ApiResponse> StatisticsToDoAsync()
    {
        // 创建一个ApiResponse对象，用于返回统计操作的结果信息
        ApiResponse apiResponse = new();

        try
        {
            var result = await _toDoInfoRepoService.StatisticsToDoAsync();
            if (result.Success)
            {
                apiResponse.ResultCode = ResultCodeEnum.Success;
                apiResponse.Msg = "统计待办事项成功";
                apiResponse.ResultData = new StatisticsToDoDto()
                {
                    Total = result.total,
                    Completed = result.completed
                };
                return apiResponse;
            }
            else
            {
                apiResponse.ResultCode = ResultCodeEnum.Error;
                apiResponse.Msg = "统计待办事项失败";
                apiResponse.ResultData = result.Code;
                return apiResponse;
            }
        }
        catch (Exception ex)
        {
            // 如果发生异常，则返回异常错误,通过响应帮助类生成异常响应信息
            apiResponse = _apiResponseHelper.GenerateErrorResponse(ex);
        }
        // 返回统计操作的结果信息
        return _apiResponseHelper.GenerateErrorResponse();
    }

    /// <summary>
    /// 更新待办事项状态
    /// </summary>
    /// <param name="id">待办事项id</param>
    /// <returns>返回一个ApiResponse，包括是否成功更新或异常信息</returns>
    public async Task<ApiResponse> UpdateToDoStatusAsync(int id)
    {
        // 创建一个ApiResponse对象，用于返回更新操作的结果信息
        ApiResponse apiResponse = new();

        try
        {
            var result = await _toDoInfoRepoService.UpdateToDoStatus(id);

            if(result == 0)
            {
                apiResponse.ResultCode = ResultCodeEnum.Success;
                apiResponse.Msg = "更新待办事项状态成功";
                apiResponse.ResultData = result;
                return apiResponse;
            }

            if ( result == 5)
            {
                apiResponse.ResultCode = ResultCodeEnum.DtoError;
                apiResponse.Msg = "服务器忙，请稍后...";
                apiResponse.ResultData = result;
                return apiResponse;
            }

            if (result == 10)
            {
                apiResponse.ResultCode = ResultCodeEnum.NotFound;
                apiResponse.Msg = "未找到该待办事项";
                apiResponse.ResultData = result;
                return apiResponse;
            }
        }
        catch (Exception ex)
        {
            // 如果发生异常，则返回异常错误,通过响应帮助类生成异常响应信息
            apiResponse = _apiResponseHelper.GenerateErrorResponse(ex);
        }
        // 返回更新操作的结果信息
        return _apiResponseHelper.GenerateErrorResponse();
    }
}

