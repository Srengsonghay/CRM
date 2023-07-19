using CRM.Models;
using CRM.Models.Dto;
using CRM.Models.Dto.ViewModel;
using Microsoft.AspNetCore.Http;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Client;
using Microsoft.Xrm.Client.Services;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Web.Helpers;
using System.Web.WebPages;

namespace CRM.Class
{
    public class CRM_DAL
    {
        public static OrganizationService service = new OrganizationService("CRMConnectionString");
        public List<Contact> ContactList()
        {
            QueryExpression query = new QueryExpression
            {
                EntityName = "contact",
                ColumnSet = new ColumnSet("contactid", "fullname", "emailaddress1", "mobilephone"),
            };
            
            OrderExpression order=new OrderExpression("fullname", OrderType.Ascending);
            query.Orders.Add(order);

            List<Contact> contactList = new List<Contact>();
            EntityCollection contactRec = service.RetrieveMultiple(query);
            if (contactRec != null && contactRec.Entities.Count > 0)
            {
                Contact ContactModel;
                contactList.Add(new Contact());
                for (int i = 0; i < contactRec.Entities.Count; i++)
                {
                    ContactModel = new Contact();
                    if (contactRec[i].Contains("contactid") && contactRec[i]["contactid"] != null)
                        ContactModel.Id = (Guid)contactRec[i]["contactid"];
                    if (contactRec[i].Contains("fullname") && contactRec[i]["fullname"] != null)
                        ContactModel.Name = contactRec[i]["fullname"].ToString();
                    if (contactRec[i].Contains("emailaddress1") && contactRec[i]["emailaddress1"] != null)
                        ContactModel.Email = contactRec[i]["emailaddress1"].ToString();
                    if (contactRec[i].Contains("mobilephone") && contactRec[i]["mobilephone"] != null)
                        ContactModel.Phone = contactRec[i]["mobilephone"].ToString();
                    contactList.Add(ContactModel);
                }
            }
            return contactList;
        }
        public async Task<CreateCaseDto> getCaseByID(Guid Id)   
        {
            try
            {
                //var Entity = new Entity("incident");
                var incidentId = Id;
                ColumnSet column = new ColumnSet("new_casetype", "prioritycode", "new_howmanyuser", "customerid", "new_portaluser", "contractid", "title", "description", "new_branchcontactname", "new_branchnumber", "contractdetailid");
                Entity incident = service.Retrieve("incident", incidentId, column);
               
                EntityReference customerId = (EntityReference)incident.Attributes["customerid"];
                EntityReference contactid = (EntityReference)incident.Attributes["new_portaluser"];
                EntityReference contractid = (EntityReference)incident.Attributes["contractid"];

                EntityReference productLineid = (EntityReference)incident.Attributes["contractdetailid"];

                CreateCaseDto getCase = new CreateCaseDto()
                {
                    Id = incident.Id,
                    CaseIssue = incident.GetAttributeValue<OptionSetValue>("new_casetype").Value,
                    PriorityCode = incident.GetAttributeValue<OptionSetValue>("prioritycode").Value,
                    newHowManyUser = incident.GetAttributeValue<OptionSetValue>("new_howmanyuser").Value,
                    CustomerId = customerId.Id,
                    PrimaryContactId = contactid.Id,
                    ContractDetailId = productLineid.Id,
                    ContractId = contractid.Id,
                    Title = (string)incident.Attributes["title"],
                    Description = (string)incident.Attributes["description"],
                    newBranchContactName = (string)incident.Attributes["new_branchcontactname"],
                    newBranchNumber = (string)incident.Attributes["new_branchnumber"],
                };
                return getCase;
            }
            catch(Exception ex)
            {
                var issue = ex.Message;
                
            }
            return null;
        }
        public string CreateCase(CreateCaseDto createCaseDto)
        {
            var CaseNo = "";
            var CaseId = "";
            try
            {
                var incident = new Entity("incident");
                //incidentId

                //This is for Integer Value 
                incident.Attributes["new_casetype"] = new OptionSetValue(createCaseDto.CaseIssue); //new OptionSetValue(2);
                incident.Attributes["casetypecode"] = new OptionSetValue(createCaseDto.CaseTypeCode);
                incident.Attributes["prioritycode"] = new OptionSetValue(createCaseDto.PriorityCode); //new OptionSetValue(3);
                incident.Attributes["new_howmanyuser"] = new OptionSetValue(createCaseDto.newHowManyUser); //new OptionSetValue(2);

                //This is for GuID Value
                EntityReference CustomerId = new EntityReference("account", createCaseDto.CustomerId);
                EntityReference ContactId = new EntityReference("contact", createCaseDto.PrimaryContactId);
                incident.Attributes["customerid"] = CustomerId; //CustomerName: First Cambodia Co., Ltd
                incident.Attributes["primarycontactid"] = ContactId;
                incident.Attributes["new_portaluser"] = ContactId;
                EntityReference ContractId = new EntityReference("contract", createCaseDto.ContractId);
                EntityReference ProductLineId = new EntityReference("contractdetail", createCaseDto.ContractDetailId);
                incident.Attributes["contractid"] = ContractId;
                incident.Attributes["contractdetailid"] = ProductLineId;

                //This is for normal text or string
                incident.Attributes["title"] = createCaseDto.Title;
                incident.Attributes["description"] = createCaseDto.Description;
                incident.Attributes["new_branchcontactname"] = createCaseDto.newBranchContactName;
                incident.Attributes["new_branchnumber"] = createCaseDto.newBranchNumber;

                var incidentId = service.Create(incident);
                CaseId = incidentId.ToString();
                var _case = service.Retrieve(incident.LogicalName, incidentId, new ColumnSet(true));
                //Case number 
                CaseNo = _case["ticketnumber"].ToString();
                if(createCaseDto.FileBody != null && createCaseDto.FileBody != "")
                {
                    Guid targetCommentId =  CreateBlankComment(incidentId);
                    var attachment = new PortalCommentAttachmentDto
                    {
                        Id = targetCommentId,
                        AdxPortalComment = createCaseDto.AdxPortalComment,
                        FileName = createCaseDto.FileName,
                        MimeType = createCaseDto.MimeType,
                        FileBody = createCaseDto.FileBody,
                    };
                    PortalCommentAttachment(attachment);
                }
            }
            catch (Exception ex)
            {
                var issue = ex.Message;
            }
            return CaseId;
            //return CaseNo;
        }
        public Guid CreateBlankComment(Guid incidentId)
        {
            Guid targetCommentId = Guid.Empty;
            try
            {
                //Create blank comment
                Entity comment = new Entity("adx_portalcomment");
                EntityReference caseId = new EntityReference("incident", incidentId);
                comment.Attributes["regardingobjectid"] = caseId;
                targetCommentId = service.Create(comment);
                return targetCommentId;
            }
            catch(Exception ex){
                var issue = ex.Message;
            }
            return targetCommentId;
        }
        public string UpdateCase(CreateCaseDto caseDto)
        {
            var CaseNo = "";
            try
            {
                var Entity = new Entity("incident");
                var incident = service.Retrieve(Entity.LogicalName, caseDto.Id, new ColumnSet(true));
                //This is for Integer Value
                incident.Attributes["new_casetype"] = new OptionSetValue(caseDto.CaseIssue);
                incident.Attributes["prioritycode"] = new OptionSetValue(caseDto.PriorityCode);
                incident.Attributes["casetypecode"] = new OptionSetValue(caseDto.CaseTypeCode);
                incident.Attributes["new_howmanyuser"] = new OptionSetValue(caseDto.newHowManyUser);

                //This is for GuID Value
                EntityReference CustomerId = new EntityReference("account", caseDto.CustomerId);
                EntityReference ContactId = new EntityReference("contact", caseDto.PrimaryContactId);
                incident.Attributes["customerid"] = CustomerId; //CustomerName: First Cambodia Co., Ltd
                incident.Attributes["primarycontactid"] = ContactId;
                incident.Attributes["new_portaluser"] = ContactId;
                EntityReference ContractId = new EntityReference("contract", caseDto.ContractId);
                EntityReference ProductLineId = new EntityReference("contractdetail", caseDto.ContractDetailId);
                incident.Attributes["contractid"] = ContractId;
                incident.Attributes["contractdetailid"] = ProductLineId;

                //This is for normal text or string
                incident.Attributes["title"] = caseDto.Title;
                incident.Attributes["description"] = caseDto.Description;
                incident.Attributes["new_branchcontactname"] = caseDto.newBranchContactName;
                incident.Attributes["new_branchnumber"] = caseDto.newBranchNumber;
                service.Update(incident);
                var _case = service.Retrieve(incident.LogicalName, caseDto.Id, new ColumnSet(true));
                CaseNo = _case["ticketnumber"].ToString();
                
            }
            catch (Exception ex)
            {
                var issue = ex.Message;
            }
            return CaseNo;
        }
        public CaseDto CancelCase(CreateCaseDto caseDto)
        {
            var Case = new CaseDto();
            try
            {
                var Entity = new Entity("incident");
                var incident = service.Retrieve(Entity.LogicalName, caseDto.Id, new ColumnSet(true));
                incident.Attributes["statecode"] = new OptionSetValue(caseDto.StateCode);
                incident.Attributes["statuscode"] = new OptionSetValue(caseDto.StatusCode);
                service.Update(incident);
                Case.IsSuccess = true;
                //statecode 2 statuscode 6
            }
            catch(Exception ex)
            {
                Case.IsSuccess = false;
                Case.ErrorMessage = ex.Message.ToString();
            }
            return Case;
        }

        public PortalComment CreatePortalComment(PortalCommentDto portalComment)
        {
            //incomming
            try
            {  
                Entity comment = new Entity("adx_portalcomment");
                EntityReference caseId = new EntityReference("incident", portalComment.CaseId);
                comment.Attributes["regardingobjectid"] = caseId;

                Entity fromparty = new Entity("activityparty");
                fromparty["partyid"] = new EntityReference("contact", portalComment.ContactId); //Incoming: contactId; Outgoing: userId;
                comment["from"] = new Entity[] { fromparty };

                Entity toparty = new Entity("activityparty");
                toparty["partyid"] = new EntityReference("systemuser", portalComment.UserId); //Incoming: userId; Outgoing: contactId;
                comment["to"] = new Entity[] { toparty };

                comment["subject"] = portalComment.Subject;
                comment["description"] = portalComment.Description;
                comment["adx_portalcommentdirectioncode"] = new OptionSetValue(portalComment.PortalCommentDirectionCode);
                //Direction: 1.Incoming; 2.Outgoing
                //service.Create(comment);

                //File Attachment
                Guid targetCommentId = service.Create(comment);
                if(portalComment.FileBody != null && portalComment.FileBody != "")
                {
                    var Attachemnt = new PortalCommentAttachmentDto
                    {
                        Id = targetCommentId,
                        AdxPortalComment = portalComment.AdxPortalComment,
                        MimeType = portalComment.MimeType,
                        FileName = portalComment.FileName,
                        FileBody = portalComment.FileBody,
                    };
                    PortalCommentAttachment(Attachemnt);
                }
                return new PortalComment
                {
                    CommentId = targetCommentId,
                };  
            }
            catch(Exception ex)
            {
                var issue = ex.ToString();
            }
            return null;
        }
        //Add Attachment to Existing Comment
        public Nullable<Guid> PortalCommentAttachment(PortalCommentAttachmentDto attachment)
        {
            try{
                Entity note = new Entity("annotation");
                note.Attributes["objectid"] = new EntityReference("adx_portalcomment", attachment.Id);
                note.Attributes["objecttypecode"] = attachment.AdxPortalComment; //10367;
                note.Attributes["mimetype"] = attachment.MimeType;
                note.Attributes["filename"] = attachment.FileName;
                note.Attributes["documentbody"] = attachment.FileBody;
                var AttachemntId = service.Create(note);
                return AttachemntId;
            }
            catch(Exception ex)
            {
                var issue = ex.ToString();
            }
            return null;
        }
        
        public CheckContactDto CreateContact(ContactDto field)
        {
            try
            {
                QueryExpression query = new QueryExpression("contact");
                query.ColumnSet.AddColumns("emailaddress1");
                query.Criteria.AddCondition("emailaddress1", ConditionOperator.Equal, field.Email);

                // Execute the query and check if any contact records were found
                EntityCollection results = service.RetrieveMultiple(query);
                if (results.Entities.Count <= 0)
                {
                    Entity contact = new Entity("contact");
                    contact["salutation"] = field.Salutation;
                    contact["firstname"] = field.FirstName;
                    contact["lastname"] = field.LastName;
                    contact["parentcustomerid"] = new EntityReference("account", field.ParentCustomerId);
                    contact["gendercode"] = new OptionSetValue(field.Gender); //1 => Male; 2 => Female; -1 => Blank
                    contact["jobtitle"] = field.JobTitle;
                    contact["emailaddress1"] = field.Email;
                    contact["birthdate"] = field.Birthday;
                    contact["department"] = field.Department;
                    contact["telephone1"] = field.BusinessPhone;
                    contact["mobilephone"] = field.MobilePhone;
                    contact["new_reporttoid"] = new EntityReference("contact", field.ReportToId);
                    contact["new_isportaluser"] = field.IsPortalUser;

                    var contactId = service.Create(contact);
                    return new CheckContactDto
                    {
                        ContactId = contactId,
                    };
                }
                else
                {
                    return new CheckContactDto
                    {
                        ErrorMessage = "User is already existed in our system.",
                    };
                        
                }
            }
            catch(Exception ex)
            {
                var issue = ex.ToString();
            }
            return new CheckContactDto
            {
                ErrorMessage = "Service down! Please try again",
            };
        }

        public CheckContactDto UpdateContact(ContactDto field)
        {
            try
            {
                var Entity = new Entity("contact");
                var contact = service.Retrieve(Entity.LogicalName, field.ContactId, new ColumnSet(true));

                contact.Attributes["salutation"] = field.Salutation;
                contact.Attributes["firstname"] = field.FirstName;
                contact.Attributes["lastname"] = field.LastName;
                contact.Attributes["gendercode"] = new OptionSetValue(field.Gender);
                contact.Attributes["birthdate"] = field.Birthday;
                contact.Attributes["department"] = field.Department;
                contact.Attributes["telephone1"] = field.BusinessPhone;
                contact.Attributes["mobilephone"] = field.MobilePhone;
                contact.Attributes["address1_name"] = field.Address;
                service.Update(contact);
            }
            catch(Exception ex)
            {
                return new CheckContactDto
                {
                    ErrorMessage = ex.Message.ToString(),
                };
            }
            return new CheckContactDto
            {
                ContactId = field.ContactId,
            };
        }
        public bool ContactUsNote (ContactUsDto model)
        {
            try
            {
                Entity contactNote = new Entity("lead");
                contactNote.Attributes["firstname"] = model.FirstName;
                contactNote.Attributes["lastname"] = model.LastName;
                contactNote.Attributes["emailaddress1"] = model.Email;
                contactNote.Attributes["mobilephone"] = model.PhoneNumber;
                contactNote.Attributes["address1_composite"] = model.Address;
                contactNote.Attributes["subject"] = model.Topic;
                contactNote.Attributes["description"] = model.Description;

                service.Create(contactNote);
                return true;
            }
            catch(Exception ex)
            {
                var issue = ex.Message.ToString();
            }
            return false;
        }
    }
}