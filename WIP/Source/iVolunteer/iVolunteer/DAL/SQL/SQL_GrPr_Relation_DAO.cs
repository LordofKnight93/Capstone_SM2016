using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using iVolunteer.Common;
using iVolunteer.Models.SQL;

namespace iVolunteer.DAL.SQL
{
    public class SQL_GrPr_Relation_DAO
    {
        /// <summary>
        /// add a relation that a group participate in a project
        /// </summary>
        /// <param name="groupID"></param>
        /// <param name="projectID"></param>
        /// <returns></returns>
        public bool Add_Joined_Group(string groupID, string projectID)
        {
            try
            {
                using (iVolunteerEntities dbEntities = new iVolunteerEntities())
                {
                    SQL_GrPr_Relation relation = new SQL_GrPr_Relation();
                    relation.GroupID = groupID;
                    relation.ProjectID = projectID;
                    relation.Relation = GrPrRelation.MEMBER_RELATION;
                    relation.Status = Status.ACCEPTED;

                    dbEntities.SQL_GrPr_Relation.Add(relation);
                    dbEntities.SaveChanges();
                    return true;
                }
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// add a relation that a group organized a project
        /// </summary>
        /// <param name="groupID"></param>
        /// <param name="projectID"></param>
        /// <returns></returns>
        public bool Add_Organized_Group(string groupID, string projectID)
        {
            try
            {
                using (iVolunteerEntities dbEntities = new iVolunteerEntities())
                {
                    SQL_GrPr_Relation relation = new SQL_GrPr_Relation();
                    relation.GroupID = groupID;
                    relation.ProjectID = projectID;
                    relation.Relation = GrPrRelation.ORGANIZE_RELATION;
                    relation.Status = Status.ACCEPTED;

                    dbEntities.SQL_GrPr_Relation.Add(relation);
                    dbEntities.SaveChanges();
                    return true;
                }
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// add a relation that a group sponsored a project
        /// </summary>
        /// <param name="groupID"></param>
        /// <param name="projectID"></param>
        /// <returns></returns>
        public bool Add_Sponsored_Group(string groupID, string projectID)
        {
            try
            {
                using (iVolunteerEntities dbEntities = new iVolunteerEntities())
                {
                    SQL_GrPr_Relation relation = new SQL_GrPr_Relation();
                    relation.GroupID = groupID;
                    relation.ProjectID = projectID;
                    relation.Relation = GrPrRelation.SPONSOR_RELATION;
                    relation.Status = Status.ACCEPTED;

                    dbEntities.SQL_GrPr_Relation.Add(relation);
                    dbEntities.SaveChanges();
                    return true;
                }
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// add join request relation
        /// </summary>
        /// <param name="groupID"></param>
        /// <param name="projectID"></param>
        /// <returns></returns>
        public bool Add_Join_Request(string groupID, string projectID)
        {
            try
            {
                using (iVolunteerEntities dbEntities = new iVolunteerEntities())
                {
                    SQL_GrPr_Relation relation = new SQL_GrPr_Relation();
                    relation.GroupID = groupID;
                    relation.ProjectID = projectID;
                    relation.Relation = GrPrRelation.MEMBER_RELATION;
                    relation.Status = Status.PENDING;

                    dbEntities.SQL_GrPr_Relation.Add(relation);
                    dbEntities.SaveChanges();
                    return true;
                }
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// add sponsor request relation
        /// </summary>
        /// <param name="groupID"></param>
        /// <param name="projectID"></param>
        /// <returns></returns>
        public bool Add_Sponsor_Request(string groupID, string projectID)
        {
            try
            {
                using (iVolunteerEntities dbEntities = new iVolunteerEntities())
                {
                    SQL_GrPr_Relation relation = new SQL_GrPr_Relation();
                    relation.GroupID = groupID;
                    relation.ProjectID = projectID;
                    relation.Relation = GrPrRelation.SPONSOR_RELATION;
                    relation.Status = Status.PENDING;

                    dbEntities.SQL_GrPr_Relation.Add(relation);
                    dbEntities.SaveChanges();
                    return true;
                }
            }
            catch
            {
                throw;
            }
        }
        /// <summary>
        /// delete a joined group
        /// </summary>
        /// <param name="groupID"></param>
        /// <param name="projectID"></param>
        /// <returns></returns>
        public bool Delete_Joined_Group(string groupID, string projectID)
        {
            try
            {
                using (iVolunteerEntities dbEntities = new iVolunteerEntities())
                {
                    var result = dbEntities.SQL_GrPr_Relation.FirstOrDefault(rl => rl.GroupID == groupID
                                                                   && rl.ProjectID == projectID
                                                                   && rl.Relation == GrPrRelation.MEMBER_RELATION
                                                                   && rl.Status == Status.ACCEPTED);
                    if (result != null)
                    {
                        dbEntities.SQL_GrPr_Relation.Remove(result);
                        dbEntities.SaveChanges();
                        return true;
                    }
                    return false;
                }
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// delete a sponsored group
        /// </summary>
        /// <param name="groupID"></param>
        /// <param name="projectID"></param>
        /// <returns></returns>
        public bool Delete_Sponsored_Group(string groupID, string projectID)
        {
            try
            {
                using (iVolunteerEntities dbEntities = new iVolunteerEntities())
                {
                    var result = dbEntities.SQL_GrPr_Relation.FirstOrDefault(rl => rl.GroupID == groupID
                                                                   && rl.ProjectID == projectID
                                                                   && rl.Relation == GrPrRelation.SPONSOR_RELATION
                                                                   && rl.Status == Status.ACCEPTED);
                    if (result != null)
                    {
                        dbEntities.SQL_GrPr_Relation.Remove(result);
                        dbEntities.SaveChanges();
                        return true;
                    }
                    return false;
                }
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// delete a organized group
        /// </summary>
        /// <param name="groupID"></param>
        /// <param name="projectID"></param>
        /// <returns></returns>
        public bool Delete_Organized_Group(string groupID, string projectID)
        {
            try
            {
                using (iVolunteerEntities dbEntities = new iVolunteerEntities())
                {
                    var result = dbEntities.SQL_GrPr_Relation.FirstOrDefault(rl => rl.GroupID == groupID
                                                                   && rl.ProjectID == projectID
                                                                   && rl.Relation == GrPrRelation.ORGANIZE_RELATION
                                                                   && rl.Status == Status.ACCEPTED);
                    if (result != null)
                    {
                        dbEntities.SQL_GrPr_Relation.Remove(result);
                        dbEntities.SaveChanges();
                        return true;
                    }
                    return false;
                }
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// delete a joined request
        /// </summary>
        /// <param name="groupID"></param>
        /// <param name="projectID"></param>
        /// <returns></returns>
        public bool Delete_Join_Request(string groupID, string projectID)
        {
            try
            {
                using (iVolunteerEntities dbEntities = new iVolunteerEntities())
                {
                    var result = dbEntities.SQL_GrPr_Relation.FirstOrDefault(rl => rl.GroupID == groupID
                                                                   && rl.ProjectID == projectID
                                                                   && rl.Relation == GrPrRelation.MEMBER_RELATION
                                                                   && rl.Status == Status.PENDING);
                    if (result != null)
                    {
                        dbEntities.SQL_GrPr_Relation.Remove(result);
                        dbEntities.SaveChanges();
                        return true;
                    }
                    return false;
                }
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// delete a sponsored request
        /// </summary>
        /// <param name="groupID"></param>
        /// <param name="projectID"></param>
        /// <returns></returns>
        public bool Delete_Sponsor_Request(string groupID, string projectID)
        {
            try
            {
                using (iVolunteerEntities dbEntities = new iVolunteerEntities())
                {
                    var result = dbEntities.SQL_GrPr_Relation.FirstOrDefault(rl => rl.GroupID == groupID
                                                                   && rl.ProjectID == projectID
                                                                   && rl.Relation == GrPrRelation.SPONSOR_RELATION
                                                                   && rl.Status == Status.PENDING);
                    if (result != null)
                    {
                        dbEntities.SQL_GrPr_Relation.Remove(result);
                        dbEntities.SaveChanges();
                        return true;
                    }
                    return false;
                }
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// accept group to join project
        /// </summary>
        /// <param name="groupID"></param>
        /// <param name="projectID"></param>
        /// <returns></returns>
        public bool Accept_Join_Request(string groupID, string projectID)
        {
            try
            {
                using (iVolunteerEntities dbEntities = new iVolunteerEntities())
                {
                    var result = dbEntities.SQL_GrPr_Relation.FirstOrDefault(rl => rl.GroupID == groupID
                                                                   && rl.ProjectID == projectID
                                                                   && rl.Relation == GrPrRelation.MEMBER_RELATION
                                                                   && rl.Status == Status.PENDING);
                    if (result != null)
                    {
                        result.Status = Status.ACCEPTED;
                        dbEntities.SaveChanges();
                        return true;
                    }
                    return false;
                }
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// accept group to sponsor project
        /// </summary>
        /// <param name="groupID"></param>
        /// <param name="projectID"></param>
        /// <returns></returns>
        public bool Accept_Sponsor_Request(string groupID, string projectID)
        {
            try
            {
                using (iVolunteerEntities dbEntities = new iVolunteerEntities())
                {
                    var result = dbEntities.SQL_GrPr_Relation.FirstOrDefault(rl => rl.GroupID == groupID
                                                                   && rl.ProjectID == projectID
                                                                   && rl.Relation == GrPrRelation.SPONSOR_RELATION
                                                                   && rl.Status == Status.PENDING);
                    if (result != null)
                    {
                        result.Status = Status.ACCEPTED;
                        dbEntities.SaveChanges();
                        return true;
                    }
                    return false;
                }
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// check if a group requested to join a project
        /// </summary>
        /// <param name="groupID"></param>
        /// <param name="projectID"></param>
        /// <returns></returns>
        public bool Is_Join_Requested(string groupID, string projectID)
        {
            try
            {
                using (iVolunteerEntities dbEntities = new iVolunteerEntities())
                {
                    var result = dbEntities.SQL_GrPr_Relation.FirstOrDefault(rl => rl.GroupID == groupID
                                                                   && rl.ProjectID == projectID
                                                                   && rl.Relation == GrPrRelation.MEMBER_RELATION
                                                                   && rl.Status == Status.PENDING);
                    return result != null;
                }
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// check if a group requested to sponsor a project
        /// </summary>
        /// <param name="groupID"></param>
        /// <param name="projectID"></param>
        /// <returns></returns>
        public bool Is_Sponsor_Requested(string groupID, string projectID)
        {
            try
            {
                using (iVolunteerEntities dbEntities = new iVolunteerEntities())
                {
                    var result = dbEntities.SQL_GrPr_Relation.FirstOrDefault(rl => rl.GroupID == groupID
                                                                   && rl.ProjectID == projectID
                                                                   && rl.Relation == GrPrRelation.SPONSOR_RELATION
                                                                   && rl.Status == Status.PENDING);
                    return result != null;
                }
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// check if a group joined a project
        /// </summary>
        /// <param name="groupID"></param>
        /// <param name="projectID"></param>
        /// <returns></returns>
        public bool Is_Joined(string groupID, string projectID)
        {
            try
            {
                using (iVolunteerEntities dbEntities = new iVolunteerEntities())
                {
                    var result = dbEntities.SQL_GrPr_Relation.FirstOrDefault(rl => rl.GroupID == groupID
                                                                   && rl.ProjectID == projectID
                                                                   && rl.Relation == GrPrRelation.MEMBER_RELATION
                                                                   && rl.Status == Status.ACCEPTED);
                    return result != null;
                }
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// check if a group is sponsor a project
        /// </summary>
        /// <param name="groupID"></param>
        /// <param name="projectID"></param>
        public bool Is_Sponsored(string groupID, string projectID)
        {
            try
            {
                using (iVolunteerEntities dbEntities = new iVolunteerEntities())
                {
                    var result = dbEntities.SQL_GrPr_Relation.FirstOrDefault(rl => rl.GroupID == groupID
                                                                   && rl.ProjectID == projectID
                                                                   && rl.Relation == GrPrRelation.SPONSOR_RELATION
                                                                   && rl.Status == Status.ACCEPTED);
                    return result != null;
                }
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// get active groups has joined a project
        /// </summary>
        /// <param name="projectID"></param>
        /// <returns></returns>
        public List<string> Get_Joined_Groups(string projectID)
        {
            try
            {
                using (iVolunteerEntities dbEntities = new iVolunteerEntities())
                {
                    var result = dbEntities.SQL_GrPr_Relation.Where(rl => rl.ProjectID == projectID
                                                                   && rl.Relation == AcPrRelation.MEMBER_RELATION
                                                                   && rl.Status == Status.ACCEPTED
                                                                   && rl.SQL_Group.IsActivate == Status.IS_ACTIVATE)
                                                             .Select(rl => rl.GroupID).ToList();
                    return result;
                }
            }
            catch
            {
                throw;
            }
        }
        /// <summary>
        /// get active groups has organized a project
        /// </summary>
        /// <param name="projectID"></param>
        /// <returns></returns>
        public List<string> Get_Organized_Groups(string projectID)
        {
            try
            {
                using (iVolunteerEntities dbEntities = new iVolunteerEntities())
                {
                    var result = dbEntities.SQL_GrPr_Relation.Where(rl => rl.ProjectID == projectID
                                                                   && rl.Relation == AcPrRelation.ORGANIZE_RELATION
                                                                   && rl.Status == Status.ACCEPTED
                                                                   && rl.SQL_Group.IsActivate == Status.IS_ACTIVATE)
                                                             .Select(rl => rl.GroupID).ToList();
                    return result;
                }
            }
            catch
            {
                throw;
            }
        }
        /// <summary>
        /// get active groups has spomnsored a project
        /// </summary>
        /// <param name="projectID"></param>
        /// <returns></returns>
        public List<string> Get_Sponsored_Groups(string projectID)
        {
            try
            {
                using (iVolunteerEntities dbEntities = new iVolunteerEntities())
                {
                    var result = dbEntities.SQL_GrPr_Relation.Where(rl => rl.ProjectID == projectID
                                                                   && rl.Relation == AcPrRelation.SPONSOR_RELATION
                                                                   && rl.Status == Status.ACCEPTED
                                                                   && rl.SQL_Group.IsActivate == Status.IS_ACTIVATE)
                                                             .Select(rl => rl.GroupID).ToList();
                    return result;
                }
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// get active groups requested to join a project
        /// </summary>
        /// <param name="projectID"></param>
        /// <returns></returns>
        public List<string> Get_Join_Requests(string projectID)
        {
            try
            {
                using (iVolunteerEntities dbEntities = new iVolunteerEntities())
                {
                    var result = dbEntities.SQL_GrPr_Relation.Where(rl => rl.ProjectID == projectID
                                                                   && rl.Relation == AcPrRelation.MEMBER_RELATION
                                                                   && rl.Status == Status.PENDING
                                                                   && rl.SQL_Group.IsActivate == Status.IS_ACTIVATE)
                                                             .Select(rl => rl.GroupID).ToList();
                    return result;
                }
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// get active groups requested sponsor a project
        /// </summary>
        /// <param name="projectID"></param>
        /// <returns></returns>
        public List<string> Get_Sponsor_Requests(string projectID)
        {
            try
            {
                using (iVolunteerEntities dbEntities = new iVolunteerEntities())
                {
                    var result = dbEntities.SQL_GrPr_Relation.Where(rl => rl.ProjectID == projectID
                                                                   && rl.Relation == AcPrRelation.SPONSOR_RELATION
                                                                   && rl.Status == Status.PENDING 
                                                                   && rl.SQL_Group.IsActivate == Status.IS_ACTIVATE)
                                                             .Select(rl => rl.GroupID).ToList();
                    return result;
                }
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// get group current active projects
        /// </summary>
        /// <param name="groupID"></param>
        public List<string> Get_Current_Projects(string groupID)
        {
            try
            {
                using (iVolunteerEntities dbEntities = new iVolunteerEntities())
                {
                    var result = dbEntities.SQL_GrPr_Relation.Where(rl => rl.GroupID == groupID
                                                                   && rl.Status == Status.ACCEPTED
                                                                   && rl.SQL_Project.IsActivate == Status.IS_ACTIVATE
                                                                   && rl.SQL_Project.InProgress == Status.ONGOING)
                                                             .Select(rl => rl.ProjectID).Distinct().ToList();
                    return result;
                }
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// get group joined active projects
        /// </summary>
        /// <param name="groupID"></param>
        public List<string> Get_Joined_Projects(string groupID)
        {
            try
            {
                using (iVolunteerEntities dbEntities = new iVolunteerEntities())
                {
                    var result = dbEntities.SQL_GrPr_Relation.Where(rl => rl.GroupID == groupID
                                                                   && rl.Relation == GrPrRelation.MEMBER_RELATION
                                                                   && rl.Status == Status.ACCEPTED
                                                                   && rl.SQL_Project.IsActivate == Status.IS_ACTIVATE
                                                                   && rl.SQL_Project.InProgress == Status.ENDED)
                                                             .Select(rl => rl.ProjectID).Distinct().ToList();
                    return result;
                }
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// get group sponsored active projects
        /// </summary>
        /// <param name="groupID"></param>
        public List<string> Get_Sponsored_Projects(string groupID)
        {
            try
            {
                using (iVolunteerEntities dbEntities = new iVolunteerEntities())
                {
                    var result = dbEntities.SQL_GrPr_Relation.Where(rl => rl.GroupID == groupID
                                                                   && rl.Relation == GrPrRelation.SPONSOR_RELATION
                                                                   && rl.Status == Status.ACCEPTED
                                                                   && rl.SQL_Project.IsActivate == Status.IS_ACTIVATE
                                                                   && rl.SQL_Project.InProgress == Status.ENDED)
                                                             .Select(rl => rl.ProjectID).Distinct().ToList();
                    return result;
                }
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// get group organized active projects
        /// </summary>
        /// <param name="groupID"></param>
        public List<string> Get_Organized_Projects(string groupID)
        {
            try
            {
                using (iVolunteerEntities dbEntities = new iVolunteerEntities())
                {
                    var result = dbEntities.SQL_GrPr_Relation.Where(rl => rl.GroupID == groupID
                                                                   && rl.Relation == GrPrRelation.ORGANIZE_RELATION
                                                                   && rl.Status == Status.ACCEPTED
                                                                   && rl.SQL_Project.IsActivate == Status.IS_ACTIVATE
                                                                   && rl.SQL_Project.InProgress == Status.ENDED)
                                                             .Select(rl => rl.ProjectID).Distinct().ToList();
                    return result;
                }
            }
            catch
            {
                throw;
            }
        }
    }
}