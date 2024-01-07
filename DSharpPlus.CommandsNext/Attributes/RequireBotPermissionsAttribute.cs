using System;
using System.Threading.Tasks;

namespace DSharpPlus.CommandsNext.Attributes;

/// <summary>
/// Defines that usage of this command is only possible when the bot is granted a specific permission.
/// </summary>
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
public sealed class RequireBotPermissionsAttribute : CheckBaseAttribute
{
    /// <summary>
    /// Gets the permissions required by this attribute.
    /// </summary>
    public Permissions Permissions { get; }

    /// <summary>
    /// Gets this check's behaviour in DMs. True means the check will always pass in DMs, whereas false means that it will always fail.
    /// </summary>
    public bool IgnoreDms { get; } = true;

    /// <summary>
    /// Defines that usage of this command is only possible when the bot is granted a specific permission.
    /// </summary>
    /// <param name="permissions">Permissions required to execute this command.</param>
    /// <param name="ignoreDms">Sets this check's behaviour in DMs. True means the check will always pass in DMs, whereas false means that it will always fail.</param>
    public RequireBotPermissionsAttribute(Permissions permissions, bool ignoreDms = true)
    {
        this.Permissions = permissions;
        this.IgnoreDms = ignoreDms;
    }

    public override async Task<bool> ExecuteCheckAsync(CommandContext ctx, bool help)
    {
        if (ctx.Guild == null)
        {
            return this.IgnoreDms;
        }

        DSharpPlus.Entities.DiscordMember bot = await ctx.Guild.GetMemberAsync(ctx.Client.CurrentUser.Id);
        if (bot == null)
        {
            return false;
        }

        if (bot.Id == ctx.Guild.OwnerId)
        {
            return true;
        }

        Permissions pbot = ctx.Channel.PermissionsFor(bot);

        return (pbot & Permissions.Administrator) != 0 ? true : (pbot & this.Permissions) == this.Permissions;
    }
}
