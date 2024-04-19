using Daily.API.ApiReponses;
using Daily.API.DataModel;
using Daily.API.Dtos;
using Daily.API.Filters;
using Daily.API.IServices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace Daily.API.Controllers;
/// <summary>
/// 待办事项接口
/// </summary>
[Route("api/[controller]/[action]")]
[ApiController]
public class ToDoController : ControllerBase
{
    /// <summary>
    /// 待办事项服务接口，用于实现待办事项的增删改查
    /// </summary>
    private readonly IToDoService _toDoService;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="toDoService">待办事项服务接口</param>
    public ToDoController(IToDoService toDoService)
    {
        _toDoService = toDoService;
    }

    /// <summary>
    ///统计待办事项，包括总数和已完成数
    /// </summary>
    /// <returns>返回一个ApiResponse，包括总数已完成数和完成百分比或异常信息</returns>
    [HttpGet]
    public async Task<ActionResult<ApiResponse>> StatisticsToDo() => Ok(await _toDoService.StatisticsToDoAsync());

    /// <summary>
    /// 获得待办状态的所有待办事项(待办中)
    /// </summary>
    /// <returns>返回一个ApiResponse，包括待办中的事项或异常信息</returns>
    [HttpGet]
    public async Task<ActionResult<ApiResponse>> GetActiveToDoList() => Ok(await _toDoService.GetActiveToDoListAsync());

    /// <summary>
    /// 获得待办状态的所有已完成事项(已完成)
    /// </summary>
    /// <returns>返回一个ApiResponse，包括已完成的事项或异常信息</returns>
    [HttpGet]
    public async Task<ActionResult<ApiResponse>> GetCompletedToDoList() => Ok(await _toDoService.GetCompletedToDoListAsync());

    /// <summary>
    /// 获得所有待办事项（待办中和已完成的）
    /// </summary>
    /// <returns>返回一个ApiResponse，包括所有待办事项或异常信息</returns>
    [HttpGet]
    public async Task<ActionResult<ApiResponse>> GetAllToDoList() => Ok(await _toDoService.GetAllToDoListAsync());

    /// <summary>
    /// 根据条件查询待办事项（按条件全部查询、按条件待办中查询、按条件已完成查询）
    /// </summary>
    /// <param name="status">0：全部；1：待办中；2：已完成</param>
    /// <param name="searchText">查询条件，是查询字符串</param>
    /// <returns>返回一个ApiResponse，包括查询的待办事项或异常信息</returns>
    [HttpGet]
    public async Task<ActionResult<ApiResponse>> GetConditionQueryToDoList(int status, string? searchText) => Ok(await _toDoService.GetConditionQueryToDoListAsync(status, searchText));

    /// <summary>
    /// 删除待办事项
    /// </summary>
    /// <param name="id">待办事项id，通过查询字符串接收</param>
    /// <returns>返回一个ApiResponse，包括删除是否成功或异常信息</returns>
    [HttpGet]
    public async Task<ActionResult<ApiResponse>> DeleteToDo(int id) => Ok(await _toDoService.DeleteToDoAsync(id));

    /// <summary>
    /// 更改待办事项状态（已完成、待办）
    /// </summary>
    /// <param name="id">待办事项id</param>
    /// <returns>返回一个ApiResponse，包括是否成功更新或异常信息</returns>
    [HttpGet]
    public async Task<ActionResult<ApiResponse>> UpdateToDoStatus(int id) => Ok(await _toDoService.UpdateToDoStatusAsync(id));

    /// <summary>
    /// 添加待办事项
    /// </summary>
    /// <param name="addToDoDto">待办事项信息Dto</param>
    /// <returns>返回一个ApiResponse，包括添加是否成功或异常信息</returns>
    [HttpPost]
    [ValidateModelState]
    public async Task<ActionResult<ApiResponse>> AddToDo(AddToDoDto addToDoDto) => Ok(await _toDoService.AddToDoAsync(addToDoDto));

    /// <summary>
    /// 编辑待变事项方法
    /// </summary>
    /// <param name="editToDoDto">编辑待办事项Dto</param>
    /// <returns>返回一个ApiResponse，包括编辑是否成功或异常信息</returns>
    [HttpPost]
    [ValidateModelState]
    public async Task<ActionResult<ApiResponse>> EditToDo(EditToDoDto editToDoDto) => Ok(await _toDoService.EditToDoAsync(editToDoDto));

    
    
}

