﻿using Microsoft.EntityFrameworkCore;
using TrueBalances.Data;
using TrueBalances.Models;
using TrueBalances.Repositories.Interfaces;

namespace TrueBalances.Repositories.DbRepositories
{
    public class UserGroupRepository : IUserGroupService
    {
        private readonly UserContext _context;

        public UserGroupRepository(UserContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Group>> GetGroupsByUserIdAsync(int userId)
        {
            return await _context.UsersGroup
                .Where(ug => ug.Id == userId)
                .Select(ug => ug.Group)
                .ToListAsync();
        }

        public async Task AddMembersToGroupAsync(int groupId, List<int> memberIds)
        {
            foreach (var memberId in memberIds)
            {
                _context.UsersGroup.Add(new UserGroup { GroupId = groupId, Id = memberId });
            }
            await _context.SaveChangesAsync();
        }

        public async Task UpdateMembersInGroupAsync(int groupId, List<int> memberIds)
        {
            var existingMembers = _context.UsersGroup.Where(ug => ug.GroupId == groupId);
            _context.UsersGroup.RemoveRange(existingMembers);
            await AddMembersToGroupAsync(groupId, memberIds);
        }

        public async Task RemoveGroupAsync(int groupId)
        {
            var userGroups = _context.UsersGroup.Where(ug => ug.GroupId == groupId);
            _context.UsersGroup.RemoveRange(userGroups);
            await _context.SaveChangesAsync();
        }
    }
}
