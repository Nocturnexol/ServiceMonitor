using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using BS.Microservice.Web.Common;
using BS.Microservice.Web.Model;
using MongoDB.Driver.Builders;

namespace BS.Microservice.Web.Areas.System.Controllers
{
    public class GroupButtonController : Controller
    {
        //
        // GET: /System/GroupButton/
        public ActionResult Index(int id)
        {
            var obj = BusinessContext.FunctionalAuthority.Get(Query<FunctionalAuthority>.EQ(t => t.Rid, id));
            ViewBag.Functional = obj;
            //获取已有的按钮
            var hasButtonIdList =
                BusinessContext.tblGroupButton.GetList(Query<tblGroupButton>.EQ(t => t.Group_NameId, id))
                    .Select(p => p.ButtonNameId)
                    .ToList();
            ViewBag.HasButtonIdList = hasButtonIdList;
            var list = BusinessContext.tblButtonName.GetList();
            return View(list);
        }

        [HttpPost]
        public ActionResult SaveData()
        {
            var rm = new ReturnMessage();

            try
            {
                var paramData = Request.Form["paramData"];
                var id = Convert.ToInt32(Request.Form["Id"]);
                // 获取设置的按钮列表
                var list = Newtonsoft.Json.JsonConvert.DeserializeObject<List<tblGroupButton>>(paramData);
                // 获取菜单原来关联的按钮
                var oldListGrpBtn =
                    BusinessContext.tblGroupButton.GetList(Query<tblGroupButton>.EQ(t => t.Group_NameId, id));
                // 保存新增的按钮
                var addListGrpBtn = new List<tblGroupButton>();

                //循环页面设置的按钮
                foreach (var model in list)
                {
                    var obj = oldListGrpBtn.FirstOrDefault(p => p.Group_NameId == model.Group_NameId && p.ButtonNameId == model.ButtonNameId);
                    if (obj == null) // 新增的按钮
                    {
                        addListGrpBtn.Add(model);
                    }
                    else // 已存在的
                    {
                        oldListGrpBtn.Remove(obj); //移除已存在的项,最终剩下的则是取消的项
                    }
                }

                // 判断是否有新增和删除
                if (addListGrpBtn.Count == 0 && oldListGrpBtn.Count == 0)
                {
                    rm.IsSuccess = true; // 没有变化
                }
                else
                {
                    // 新增的按钮
                    //foreach (tblGroupButton item in add_list_grpBtn)
                    //{
                    //    string a = "insert into tblGroupButton(Group_NameId,ButtonNameId,Remark) values (" + item.Group_NameId + "," + item.ButtonNameId + ",'" + item.Remark + "')";
                    //    listSql.Add(a);
                    //    //db.tblGroupButton.AddObject(item);
                    //}
                    var flag = BusinessContext.tblGroupButton.Add(addListGrpBtn);
                    //foreach (tblGroupButton item in old_list_grpBtn)
                    //{
                    //    string a = "delete from tblGroupButton where Rid = " + item.Rid + "";
                    //    listSql.Add(a);
                    //}
                    flag = flag && BusinessContext.tblGroupButton.Delete(oldListGrpBtn.Select(t => t.Rid).ToList());
                    //// 取消的按钮
                    var delGrpbtnIdList = oldListGrpBtn.Select(p => p.Rid).ToList();
                    if (delGrpbtnIdList.Count > 0)
                    {
                        var q = Query.And(Query<sys_role_right>.In(t => t.rf_Right_Code, delGrpbtnIdList),
                            Query<sys_role_right>.EQ(t => t.rf_Type, "数据管理"));
                        var oldListRoleRight = BusinessContext.sys_role_right.GetList(q);
                        //foreach (sys_role_right item in old_list_roleRight)
                        //{
                        //    string a = "delete from sys_role_right where Rid = " + item.Rid + "";
                        //    listSql.Add(a);
                        //}
                        flag = flag && BusinessContext.sys_role_right.Delete(oldListRoleRight.Select(t => t.Rid).ToList());
                    }
                    if (flag)
                    {
                        rm.IsSuccess = true;
                        rm.Message = "删除成功！";
                    }
                    else
                    {
                        rm.IsSuccess = false;
                        rm.Message = "删除失败！";
                    }
                }
            }
            catch (Exception ex)
            {

                rm.IsSuccess = false;
                rm.Message = ex.Message;
            }
            return Json(rm, JsonRequestBehavior.AllowGet);
        }
    }
}
