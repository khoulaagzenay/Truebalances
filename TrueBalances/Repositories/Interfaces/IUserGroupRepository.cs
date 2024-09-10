﻿using TrueBalances.Models;

namespace TrueBalances.Repositories.Interfaces
{
    public interface IUserGroupRepository
    {
        Task<IEnumerable<Group>> GetGroupsByUserIdAsync(string userId);
        public bool UserIsInGroup(string userId, int groupId);
        Task AddMembersToGroupAsync(int groupId, List<string> memberIds);
        Task UpdateMembersInGroupAsync(int groupId, List<string> memberIds);
        Task RemoveGroupAsync(int groupId);

        //Task<List<CustomUser>> GetAllUsersAsync();
    }
}