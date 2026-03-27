using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace i2e1_core.Models
{
    public static class SupportTicketConstants
    {
        // Ticket Types
        public const string RECHARGE_ISP = "ISP";
        public const string ROUTER_PICKUP = "PICKUP";
        public const string INCENTIVE = "INCENTIVE";
        public const string WITHDRAWALREQUEST = "WITHDRAWALREQUEST";

        // Ticket Statuses
        public const int TODO = 0;
        public const int IN_PROGRESS = 1;
        public const int RESOLVED = 2;
        // Ticket Priority
        public const int LOW = 0;
        public const int MEDIUM = 1;
        public const int HIGH = 2;
    }
    public class SupportTicket
    {
        public long Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int Priority { get; set; }
        public long Reporter { get; set; }
        public long AssignedTo { get; set; }
        public string Mobile { get; set; }
        public long PartnerId { get; set; }
        public string? Type { get; set; }
        public DateTime? DueDate { get; set; }
        public string? Configs { get; set; }
        public DateTime Created { get; set; }
        public DateTime Modified { get; set; }
        public int Status { get; set; }

        public string ExtraData { get; set; }
    }
    public class TaskAttachment
    {
        public long Id { get; set; }
        public long TaskId { get; set; }
        public string FileName { get; set; }
        public string FileType { get; set; }
        public int Status { get; set; }
    }
    public static class TaskFileTypes
    {
        // Ticket Types
        public const string IMAGE = "IMAGE";
        public const string VIDEO = "VIDEO";
    }
 }
