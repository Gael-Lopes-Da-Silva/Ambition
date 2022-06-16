/* 
@author: Gael Lopes Da Silva
@project: Ambition Discord Bot
@github: https://github.com/Gael-Lopes-Da-Silva/Ambition
*/

using System.Globalization;
using System.Reflection;
using Discord;
using Discord.Interactions;
using Discord.Rest;
using Discord.WebSocket;

namespace Ambition;

public class Bot : InteractionModuleBase
{
    private readonly string[] bannedWords = new string[]
    {
        "卐",
        "🖕"
    };
    private readonly string[][] activityList = new string[][]
    {
        new string[]
        {
            "faire chier son monde.",
            "brûler du PQ.",
            "sauter sur son lit.",
            "comprendre son existence.",
            "faire peur aux fourmis.",
            "conduire dangereusement.",
            "s'auto coder.",
            "des jeux pas très net.",
            "invoquer un démon.",
            "être autiste.",
            "mordre les gens",
            "faire un petit personnage.",
            "creuser un trou.",
            "se cacher derrière toi.",
            "prendre des photos de toi.",
            "manger des croquettes.",
            "se jeter dans les escaliers.",
            "rigoler tout seul.",
            "manger son micro-ondes.",
            "boire de la javel.",
            "comploter contre Palindrome.",
            "se claquer la tête sur le sol.",
            "s'occuper de son pain de compagnie.",
            "démonter ses pieds.",
            "colorier de manière immonde comme un gosse de 5 ans.",
            "observer les gens dormir de manière bizarre.",
            "faire des bruits bizarre.",
            "la dînette sa mère.",
            "faire une pose en T.",
            "5 nuits chez Frédérique.",
            "parmis-nous.",
            "trop plein de lavage, en partie rapide."
        },
        new string[]
        {
            "le soleil fixement.",
            "les gens passer dehors.",
            "un documentaire sur ARTE.",
            "à travers le sol en pensant trouver un mineshaft.",
            "à l'intérieur de ses yeux.",
            "la marée en pensant Risitas.",
            "dans son sac magique.",
            "moi quand je te parle !",
            "moi tout ça ...",
            "moi bien, mon petit.",
            "les nuages.",
            "le ciel de ses morts.",
            "un gros en T pose.",
            "de l'ASMR des yeux.",
            "des dessins animés sur Gulli.",
            "la météo de toobo le bonobo.",
            "les infos sur BFMTV.",
            "4 personnes marcher sur des railles.",
            "les, tous ces cons !",
            "les deux lunes dans le ciel.",
            "une voiture brûler.",
            "l'avenir de l'humanité.",
            "l'ombre d'une personne dans le noire.",
            "le progressisme, sans âme."
        },
        new string[]
        {
            "le vent.",
            "les gens respirer.",
            "les oiseaux hurler à la mort.",
            "des bruits de pieds.",
            "un autiste hurler.",
            "Erika en pensant au bon vieux temps.",
            "el niño.",
            "un débat d'Éric Zemmour.",
            "Bury The Light ultra fort.",
            "moi, je vais pas me répéter.",
            "moi quand je te parle !",
            "des bruits d'aspirateur.",
            "les gens hurler dans sont placard.",
            "la boite à musique du tapis.",
            "les dégénérés de Marseille se battre dans la rue.",
            "les anglais éclater le stade de France.",
            "le fish au chocolat.",
            "les voix dans sa tête.",
            "des solos de triangles."
        }
    };
    private readonly ulong[] blackList = new ulong[]
    {
        407630012188196875,
        314789741155319818,
        360827777924071434,
        317364705146568704
    };

    public enum Serveur
    {
        Principal,
        Staff
    }

    private readonly DiscordSocketConfig clientConfig;
    private readonly DiscordSocketClient client;
    private readonly InteractionService interactionService;
    private readonly InteractionHandler interactionHandler;

    private static bool raidMode = false;
    private static bool isClearing = false;

    /* ---------------------------------- */

    public Bot()
    {
        clientConfig = new DiscordSocketConfig()
        {
            MessageCacheSize = 250,
            GatewayIntents = GatewayIntents.All
        };
        client = new DiscordSocketClient(clientConfig);
        interactionService = new InteractionService(client.Rest);
        interactionHandler = new InteractionHandler(client, interactionService);

        client.Log += OnLog;
        client.Ready += OnReady;
    }

    public async Task RunBot()
    {
        await interactionHandler.InitializeAsync();
        await client.LoginAsync(TokenType.Bot, "your token");
        await client.StartAsync();
        await Task.Delay(Timeout.Infinite);
    }

    /* ---------------------------------- */

    private Task OnLog(LogMessage message)
    {
        Console.WriteLine(message.ToString());
        return Task.CompletedTask;
    }

    private async Task OnReady()
    {
        await Log("--------------- Prêt ! ---------------");
        await Log($"> name:             {client.CurrentUser.Username}");
        await Log($"> id:               {client.CurrentUser.Id}");
        await Log($"> verified:         {client.CurrentUser.IsVerified}");
        await Log($"> premium:          {client.CurrentUser.PremiumType}");
        await Log($"> created at:       {client.CurrentUser.CreatedAt.DateTime}");
        await Log("--------------------------------------");

        client.MessageReceived += OnMessageReceived;
        client.MessageUpdated += OnMessageUpdated;
        client.MessageDeleted += OnMessageDeleted;
        client.UserJoined += OnUserJoined;
        client.UserLeft += OnUserLeft;
        client.UserUpdated += OnUserUpdated;
        client.GuildMemberUpdated += OnMemberUpdated;
        client.PresenceUpdated += OnPresenceUpdated;
        client.UserBanned += OnUserBanned;
        client.UserUnbanned += OnUserUnbanned;
        client.UserVoiceStateUpdated += OnVoiceStateUpdated;
        client.ChannelDestroyed += OnChannelDeleted;
        client.ChannelCreated += OnChannelCreated;
        client.ChannelUpdated += OnChannelUpdated;
        client.GuildUpdated += OnGuildUpdated;
        client.RoleCreated += OnRoleCreated;
        client.RoleDeleted += OnRoleDeleted;
        client.RoleUpdated += OnRoleUpdated;
        client.InviteCreated += OnInviteCreated;
        client.InviteDeleted += OnInviteDeleted;
        client.ThreadCreated += OnThreadCreated;
        client.ThreadDeleted += OnThreadDeleted;
        client.ThreadUpdated += OnThreadUpdated;
        client.ThreadMemberJoined += OnThreadUserJoined;
        client.ThreadMemberLeft += OnThreadUserLeft;
        client.GuildScheduledEventCreated += OnGuildEventCreated;
        client.GuildScheduledEventCompleted += OnGuildEventCompleted;
        client.GuildScheduledEventCancelled += OnGuildEventCancelled;
        client.GuildScheduledEventStarted += OnGuildEventStarted;
        client.GuildScheduledEventUpdated += OnGuildEventUpdated;
        client.GuildScheduledEventUserAdd += OnGuildEventUserAdded;
        client.GuildScheduledEventUserRemove += OnGuildEventUserRemoved;
        client.GuildStickerCreated += OnGuildStickerCreated;
        client.GuildStickerDeleted += OnGuildStickerDeleted;
        client.GuildStickerUpdated += OnGuildStickerUpdated;
        client.ButtonExecuted += OnButtonExecuted;

        _ = Task.Run(() => UpdateAvatar(3600)); // 1 hour
        _ = Task.Run(() => UpdateActivity(3600)); // 1 hour
        _ = Task.Run(() => UpdateLog(345600)); // 4 days

        await client.SetStatusAsync(UserStatus.DoNotDisturb);
    }

    private async Task OnMessageReceived(SocketMessage message)
    {
        if (message == null) return;
        if (message.Author.IsBot) return;

        if (message.Channel is IDMChannel)
        {
            var embed = new EmbedBuilder();
            embed.Title = "Est-ce que je te parle, moi ?! Alors me parle pas !";
            embed.Description = "**Alors... Tu peux exécuter les commandes du bot, mais que sur Palindrome ou Palindrome Staff. Au cas où t'es con, voici le lien du serveur principal https://discord.gg/B4hzjSrVCJ. Et si tu veux pas venir, bah fais pas chier et envois pas de message en fait. Cheh !**";
            embed.Color = new Color(0x2f3136);

            await message.Author.SendMessageAsync(embed: embed.Build());
            return;
        }

        await VerifyBadWords(message);
        await AddReactions(message);
    }

    private async Task OnMessageUpdated(Cacheable<IMessage, ulong> cachedBefore, SocketMessage after, ISocketMessageChannel channel)
    {
        if (after == null) return;
        if (channel == null) return;
        if (after.Author.IsBot) return;

        var before = await cachedBefore.GetOrDownloadAsync();
        if (before == null) return;

        if (channel is SocketGuildChannel guildChannel)
        {
            var auditLogs = await guildChannel.Guild.GetAuditLogsAsync(1, actionType: ActionType.MessagePinned).FlattenAsync();

            foreach (var audit in auditLogs)
            {
                if (before.IsPinned != after.IsPinned)
                {
                    if (audit.User.Id == after.Author.Id)
                    {
                        await Log($"> {audit.User.Username} ({audit.User.Id}) a épinglé sont message dans le salon {channel.Name}.");
                        await Log($"  * Message: {after.Content}");
                    }
                    else
                    {
                        await Log($"> {audit.User.Username} ({audit.User.Id}) a épinglé le message de {after.Author.Username} ({after.Author.Id}) dans le salon {channel.Name}.");
                        await Log($"  * Message: {after.Content}");
                    }
                }
            }
        }

        await VerifyBadWords(after);
        await AddReactions(after);
    }

    private async Task OnMessageDeleted(Cacheable<IMessage, ulong> cachedMessage, Cacheable<IMessageChannel, ulong> cachedChannel)
    {
        var message = await cachedMessage.GetOrDownloadAsync();
        var channel = await cachedChannel.GetOrDownloadAsync();
        if (channel == null) return;
        if (isClearing) return;

        if (channel is SocketGuildChannel guildChannel)
        {
            if (message != null)
            {
                var auditLogs = await guildChannel.Guild.GetAuditLogsAsync(1, actionType: ActionType.MessageDeleted).FlattenAsync();

                foreach (var audit in auditLogs)
                {
                    await Log($"> Le message de {message.Author.Username} ({message.Author.Id}) a était effacé dans le salon {channel.Name}.");
                    await Log($"  * {audit.User.Username} ({audit.User.Id}) est probablement celui qui a fait ça.");

                    if (message.Attachments.Count != 0 && message.Content.Length == 0)
                    {
                        foreach (var attachment in message.Attachments)
                        {
                            await Log($"  * Contenu: {attachment.Url}");
                        }
                    }
                    else if (message.Attachments.Count != 0 && message.Content.Length != 0)
                    {
                        await Log($"  * Message: {message.Content}");

                        foreach (var attachment in message.Attachments)
                        {
                            await Log($"  * Contenu: {attachment.Url}");
                        }
                    }
                    else
                    {
                        await Log($"  * Message: {message.Content}");
                    }
                }
            }
            else
            {
                var auditLogs = await guildChannel.Guild.GetAuditLogsAsync(1, actionType: ActionType.MessageDeleted).FlattenAsync();

                foreach (var audit in auditLogs)
                {
                    await Log($"> Un message non sauvegardé a était effacé dans dans le salon {channel.Name}.");
                    await Log($"  * {audit.User.Username} ({audit.User.Id}) est probablement celui qui a fait ça.");
                }
            }
        }
    }

    private async Task OnUserJoined(SocketGuildUser member)
    {
        if (member == null) return;
        if (member.IsBot) return;

        if (raidMode)
        {
            var embed = new EmbedBuilder();
            embed.Title = "Le serveur est actuellement en mode raid.";
            embed.Description = "Tu as été expulsé, cependant, tu peux ressayer plus tard, quand le serveur ne sera plus en mode raid.";
            embed.Color = new Color(0xff0000);

            await member.SendMessageAsync(embed: embed.Build());
            await member.KickAsync("Serveur en mode raid.");
            return;
        }

        if (member.Guild.Id == 501824700486516766)
        {
            await Log($"> {member.Username} ({member.Id}) a rejoint le serveur principal.");

            var embed = new EmbedBuilder();
            embed.Title = "Bienvenue sur Palindrome !";
            embed.ImageUrl = "https://tenor.com/view/bienvenue-welcome-gif-14086544";
            embed.Color = new Color(0x2f3136);

            await member.SendMessageAsync(embed: embed.Build());

            if (blackList.Contains(member.Id))
            {
                var warningEmbed = new EmbedBuilder();
                warningEmbed.ImageUrl = "https://cdn.discordapp.com/attachments/594977170850447411/861928219162312744/unknown.png";
                warningEmbed.Color = new Color(0x2f3136);

                await member.SendMessageAsync(embed: warningEmbed.Build());
            }

            var welcomeEmbed = new EmbedBuilder();
            welcomeEmbed.Title = "`Une nouvelle personne à rejoindre le serveur.`";
            welcomeEmbed.Description = $"**Dites bonjour à {member.Mention} !**";
            welcomeEmbed.ThumbnailUrl = member.GetAvatarUrl();
            welcomeEmbed.Color = new Color(0x2f3136);

            var channel = member.Guild.GetTextChannel(534443162949910537);
            await channel.SendMessageAsync(embed: welcomeEmbed.Build());
        }
        else if (member.Guild.Id == 583414864677175306)
        {
            await Log($"> {member.Username} ({member.Id}) a rejoint le serveur staff.");

            var embed = new EmbedBuilder();
            embed.Title = "Bienvenue sur Palindrome Staff !";
            embed.Description = "**C'est dans le salon <#649366479585673237> que tu pourra faire tes demandes. Pour faire une demande valide, il suffit de créer un fil portant le nom de la demande, comme l'image ci-dessous.**";
            embed.ImageUrl = "https://cdn.discordapp.com/attachments/594977170850447411/986220971939754044/unknown.png";
            embed.Color = new Color(0x2f3136);

            await member.SendMessageAsync(embed: embed.Build());
        }

        await VerifyUsername(member);
    }

    private async Task OnUserLeft(SocketGuild guild, SocketUser user)
    {
        if (guild == null) return;
        if (user == null) return;
        if (user.IsBot) return;
        if (raidMode) return;

        if (guild.Id == 501824700486516766)
        {
            await Log($"> {user.Username} ({user.Id}) a quitté le serveur principal.");

            var embed = new EmbedBuilder();
            embed.ImageUrl = "https://cdn.discordapp.com/attachments/594977170850447411/861928219162312744/unknown.png";
            embed.Color = new Color(0x2f3136);

            await user.SendMessageAsync(embed: embed.Build());
        }
        else if (guild.Id == 583414864677175306)
        {
            await Log($"> {user.Username} ({user.Id}) a quitté le serveur staff.");
        }
    }

    private async Task OnUserUpdated(SocketUser before, SocketUser after)
    {
        if (before == null) return;
        if (after == null) return;
        if (after.IsBot) return;

        if (after.Username != before.Username)
        {
            await Log($"> {after.Username} ({after.Id}) a changé de nom.");
            await Log($"  * Context: {before.Username} -> {after.Username}");
        }

        if (after.AvatarId != before.AvatarId)
        {
            await Log($"> {after.Username} ({after.Id}) a changé d'avatar.");
            await Log($"  * Context: {before.GetAvatarUrl()} -> {after.GetAvatarUrl()}");
        }

        if (after is SocketGuildUser user)
        {
            await VerifyUsername(user);
        }
    }

    private async Task OnMemberUpdated(Cacheable<SocketGuildUser, ulong> cachedBefore, SocketGuildUser after)
    {
        if (after == null) return;
        if (after.IsBot) return;

        var before = await cachedBefore.GetOrDownloadAsync();
        if (before == null) return;

        if (after.Nickname != before.Nickname)
        {
            await Log($"> {after.Username} ({after.Id}) a changé sont pseudo dans {after.Guild.Name}.");
            await Log($"  * Context: {before.Nickname} -> {after.Nickname}");
        }

        if (after.GuildAvatarId != before.GuildAvatarId)
        {
            await Log($"> {after.Username} ({after.Id}) a changé sont avatar dans {after.Guild.Name}.");
            await Log($"  * Context: {before.GetGuildAvatarUrl()} -> {after.GetGuildAvatarUrl()}");
        }

        if (after.Roles != before.Roles)
        {
            await Log($"> {after.Username} ({after.Id}) a vu ses rôles changer:");
            await Log("  * Avant:");
            foreach (var role in before.Roles) await Log($"  * {role.Name}");
            await Log("  * -------------------");
            await Log("  * Après:");
            foreach (var role in after.Roles) await Log($"  * {role.Name}");
        }

        await VerifyUsername(after);
    }

    private async Task OnPresenceUpdated(SocketUser user, SocketPresence before, SocketPresence after)
    {
        if (user == null) return;
        if (before == null) return;
        if (after == null) return;
        if (user.IsBot) return;

        if (user is SocketGuildUser guildUser && guildUser.Guild.Id == 501824700486516766)
        {
            if (after.Status != before.Status)
            {
                await Log($"> {user.Username} ({user.Id}) a changé de statut.");
                await Log($"  * Context: {before.Status.ToString()} -> {after.Status.ToString()}");
            }
        }
    }

    private async Task OnUserBanned(SocketUser user, SocketGuild guild)
    {
        if (user == null) return;
        if (guild == null) return;
        if (user.IsBot) return;

        var auditLogs = await guild.GetAuditLogsAsync(1, actionType: ActionType.Ban).FlattenAsync();

        foreach (var audit in auditLogs)
        {
            await Log($"> {audit.User.Username} ({audit.User.Id}) a bannit {user.Username} de {guild.Name}.");
        }
    }

    private async Task OnUserUnbanned(SocketUser user, SocketGuild guild)
    {
        if (user == null) return;
        if (guild == null) return;
        if (user.IsBot) return;

        var auditLogs = await guild.GetAuditLogsAsync(1, actionType: ActionType.Unban).FlattenAsync();

        foreach (var audit in auditLogs)
        {
            await Log($"> {audit.User.Username} ({audit.User.Id}) a débannit {user.Username} de {guild.Name}.");
        }
    }

    private async Task OnVoiceStateUpdated(SocketUser user, SocketVoiceState before, SocketVoiceState after)
    {
        if (user == null) return;

        if (after.VoiceChannel != null && after.VoiceChannel.Id == 612970606799093760)
        {
            await Log($"> {user.Username} ({user.Id}) a rejoint un salon vocal.");

            var personalChannel = await after.VoiceChannel.Guild.CreateVoiceChannelAsync($"🔈●{user.Username}●", x => x.CategoryId = 501839906952445953);

            if (user is SocketGuildUser guildUser)
            {
                await guildUser.ModifyAsync(x => x.Channel = personalChannel);
            }
        }

        if (before.VoiceChannel != null && before.VoiceChannel.Id != 612970606799093760)
        {
            if (before.VoiceChannel.ConnectedUsers.Count != 0) return;
            
            await Log($"> {user.Username} ({user.Id}) a quitté un salon vocal.");
            await before.VoiceChannel.DeleteAsync();
        }
    }

    private async Task OnChannelDeleted(SocketChannel channel)
    {
        if (channel == null) return;

        if (channel is SocketGuildChannel guildChannel)
        {
            var auditLogs = await guildChannel.Guild.GetAuditLogsAsync(1, actionType: ActionType.ChannelDeleted).FlattenAsync();

            foreach (var audit in auditLogs)
            {
                if (audit.User.IsBot) return;
                
                if (channel is ICategoryChannel)
                {
                    await Log($"> {audit.User.Username} ({audit.User.Id}) a supprimé la catégorie {guildChannel.Name} de {guildChannel.Guild.Name}.");
                }
                else
                {
                    await Log($"> {audit.User.Username} ({audit.User.Id}) a supprimé le salon {guildChannel.Name} de {guildChannel.Guild.Name}.");
                }
            }
        }
    }

    private async Task OnChannelCreated(SocketChannel channel)
    {
        if (channel == null) return;

        if (channel is SocketGuildChannel guildChannel)
        {
            var auditLogs = await guildChannel.Guild.GetAuditLogsAsync(1, actionType: ActionType.ChannelCreated).FlattenAsync();

            foreach (var audit in auditLogs)
            {
                if (audit.User.IsBot) return;
                
                if (channel is ICategoryChannel)
                {
                    await Log($"> {audit.User.Username} ({audit.User.Id}) a crée une catégorie {guildChannel.Name} dans {guildChannel.Guild.Name}.");
                }
                else
                {
                    await Log($"> {audit.User.Username} ({audit.User.Id}) a crée un salon {guildChannel.Name} dans {guildChannel.Guild.Name}.");
                }
            }
        }
    }

    private async Task OnChannelUpdated(SocketChannel before, SocketChannel after)
    {
        if (before == null) return;
        if (after == null) return;

        if (after is SocketGuildChannel guildAfter && before is SocketGuildChannel guildBefore)
        {
            var auditLogs = await guildAfter.Guild.GetAuditLogsAsync(1, actionType: ActionType.ChannelUpdated).FlattenAsync();

            foreach (var audit in auditLogs)
            {
                if (audit.User.IsBot) return;
                
                if (guildAfter.Name != guildBefore.Name && after is ICategoryChannel)
                {
                    await Log($"> {audit.User.Username} ({audit.User.Id}) a modifié le nom de la catégorie {guildAfter.Name} dans {guildAfter.Guild.Name}.");
                    await Log($"  * Context: {guildBefore.Name} -> {guildAfter.Name}");
                }
                else if (guildAfter.Name != guildBefore.Name)
                {
                    await Log($"> {audit.User.Username} ({audit.User.Id}) a modifié le nom du salon {guildAfter.Name} dans {guildAfter.Guild.Name}.");
                    await Log($"  * Context: {guildBefore.Name} -> {guildAfter.Name}");
                }
            }
        }
    }

    private async Task OnGuildUpdated(SocketGuild before, SocketGuild after)
    {
        if (before == null) return;
        if (after == null) return;

        var auditLogs = await after.GetAuditLogsAsync(1, actionType: ActionType.GuildUpdated).FlattenAsync();

        foreach (var audit in auditLogs)
        {
            if (audit.User.IsBot) return;
            
            if (after.Name != before.Name)
            {
                await Log($"> {audit.User.Username} ({audit.User.Id}) a modifié le nom de {before.Name}.");
                await Log($"  * Context: {before.Name} -> {after.Name}");
            }

            if (after.BannerId != after.BannerId)
            {
                await Log($"> {audit.User.Username} ({audit.User.Id}) a modifié la bannière de {before.Name}.");
                await Log($"  * Context: {before.BannerUrl} -> {after.BannerUrl}");
            }

            if (after.IconId != before.IconId)
            {
                await Log($"> {audit.User.Username} ({audit.User.Id}) a modifié l'icon de {before.Name}.");
                await Log($"  * Context: {before.IconUrl} -> {after.IconUrl}");
            }

            if (after.Description != before.Description)
            {
                await Log($"> {audit.User.Username} ({audit.User.Id}) a modifié la description de {before.Name}.");
                await Log($"  * Avant: {before.Description}");
                await Log($"  * Après: {after.Description}");
            }
        }

        if (after.Owner != before.Owner)
        {
            await Log($"> Le propriétaire de {before.Name} a changé.");
            await Log($"  * Context: {before.Owner.Username} ({before.Owner.Id}) -> {after.Owner.Username} ({after.Owner.Id})");
        }

        if (after.MemberCount != before.MemberCount)
        {
            if (before.MemberCount > after.MemberCount)
            {
                await Log($"> Le nombre de personnes sur {before.Name} a diminué.");
                await Log($"  * Context: {before.MemberCount} -> {after.MemberCount}");
            }
            else if (before.MemberCount < after.MemberCount)
            {
                await Log($"> Le nombre de personnes sur {before.Name} a augmenté.");
                await Log($"  * Context: {before.MemberCount} -> {after.MemberCount}");
            }
        }
    }

    private async Task OnRoleCreated(SocketRole role)
    {
        if (role == null) return;

        var auditLogs = await role.Guild.GetAuditLogsAsync(1, actionType: ActionType.RoleCreated).FlattenAsync();

        foreach (var audit in auditLogs)
        {
            if (audit.User.IsBot) return;
            
            await Log($"> {audit.User.Username} ({audit.User.Id}) a crée le role {role.Name}.");
            await Log($"  * Nom: {role.Name}");
            await Log($"  * Couleur: {role.Color.ToString()}");
            await Log($"  * Serveur: {role.Guild.Name}");
            await Log($"  * Mentionable: {role.IsMentionable}");
        }
    }

    private async Task OnRoleDeleted(SocketRole role)
    {
        if (role == null) return;

        var auditLogs = await role.Guild.GetAuditLogsAsync(1, actionType: ActionType.RoleDeleted).FlattenAsync();

        foreach (var audit in auditLogs)
        {
            if (audit.User.IsBot) return;
            
            await Log($"> {audit.User.Username} ({audit.User.Id}) a supprimé le role {role.Name}.");
            await Log($"  * Nom: {role.Name}");
            await Log($"  * Couleur: {role.Color.ToString()}");
            await Log($"  * Serveur: {role.Guild.Name}");
            await Log($"  * Mentionable: {role.IsMentionable}");
        }
    }

    private async Task OnRoleUpdated(SocketRole before, SocketRole after)
    {
        if (before == null) return;
        if (after == null) return;

        var auditLogs = await after.Guild.GetAuditLogsAsync(1, actionType: ActionType.RoleUpdated).FlattenAsync();

        foreach (var audit in auditLogs)
        {
            if (audit.User.IsBot) return;
            
            if (after.Name != before.Name)
            {
                await Log($"> {audit.User.Username} ({audit.User.Id}) a modifié le nom de {before.Name} dans {before.Guild.Name}.");
                await Log($"  * Context: {before.Name} -> {after.Name}");
            }

            if (after.Color != before.Color)
            {
                await Log($"> {audit.User.Username} ({audit.User.Id}) a modifié la couleur de {before.Name} dans {before.Guild.Name}.");
                await Log($"  * Context: {before.Color.ToString()} -> {after.Color.ToString()}");
            }

            if (after.Icon != before.Icon)
            {
                await Log($"> {audit.User.Username} ({audit.User.Id}) a modifié l'icon de {before.Name} dans {before.Guild.Name}.");
                await Log($"  * Context: {before.GetIconUrl()} -> {after.GetIconUrl()}");
            }

            if (after.IsMentionable != before.IsMentionable)
            {
                if (after.IsMentionable)
                {
                    await Log($"> {audit.User.Username} ({audit.User.Id}) a rendu le role {before.Name} mentionable dans {before.Guild.Name}.");
                }
                else
                {
                    await Log($"> {audit.User.Username} ({audit.User.Id}) a rendu le role {before.Name} non mentionable dans {before.Guild.Name}.");
                }
            }
        }
    }

    private async Task OnInviteCreated(SocketInvite invite)
    {
        if (invite == null) return;
        if (invite.Inviter.IsBot) return;

        await Log($"> {invite.Inviter.Username} ({invite.Inviter.Id}) a crée une invitation dans le salon {invite.Channel.Name} de {invite.Guild.Name}.");
        await Log($"  * Serveur: {invite.Guild.Name}");
        await Log($"  * Salon: {invite.Channel.Name}");
        await Log($"  * Lien: {invite.Url}");
        await Log($"  * Code: {invite.Code}");
        await Log($"  * Temporaire: {invite.IsTemporary}");
        await Log($"  * Expirations: {invite.MaxAge}");
        await Log($"  * Utilisations maximum: {invite.MaxUses}");
    }

    private async Task OnInviteDeleted(SocketGuildChannel channel, string code)
    {
        if (channel == null) return;

        var auditLogs = await channel.Guild.GetAuditLogsAsync(1, actionType: ActionType.InviteDeleted).FlattenAsync();

        foreach (var audit in auditLogs)
        {
            if (audit.User.IsBot) return;
            
            await Log($"> {audit.User.Username} ({audit.User.Id}) a supprimé une invitation dans le salon {channel.Name} de {channel.Guild.Name}.");

            if (audit.Data is InviteDeleteAuditLogData invite)
            {
                await Log($"  * Createur: {invite.Creator.Username} ({invite.Creator.Id})");
                await Log($"  * Serveur: {channel.Guild.Name}");
                await Log($"  * Salon: {channel.Name}");
                await Log($"  * Code: {invite.Code}");
                await Log($"  * Temporaire: {invite.Temporary}");
                await Log($"  * Expirations: {invite.MaxAge}");
                await Log($"  * Utilisations: {invite.Uses}");
                await Log($"  * Utilisations maximum: {invite.MaxUses}");
            }
        }
    }

    private async Task OnThreadCreated(SocketThreadChannel threadChannel)
    {
        if (threadChannel == null) return;

        await Log($"> {threadChannel.Owner.Username} ({threadChannel.Owner.Id}) a crée un fil dans le salon {threadChannel.ParentChannel.Name} de {threadChannel.Guild.Name}.");
        await Log($"  * Createur: {threadChannel.Owner.Username}");
        await Log($"  * Serveur: {threadChannel.Guild.Name}");
        await Log($"  * Salon: {threadChannel.ParentChannel.Name}");
        await Log($"  * Nom: {threadChannel.Name}");
        await Log($"  * NSFW: {threadChannel.IsNsfw}");
        await Log($"  * Bloqué: {threadChannel.IsLocked}");
        await Log($"  * Privée: {threadChannel.IsPrivateThread}");
    }

    private async Task OnThreadDeleted(Cacheable<SocketThreadChannel, ulong> cachedThreadChannel)
    {
        var threadChannel = await cachedThreadChannel.GetOrDownloadAsync();
        if (threadChannel == null) return;

        var auditLogs = await threadChannel.Guild.GetAuditLogsAsync(1, actionType: ActionType.ThreadDelete).FlattenAsync();

        foreach (var audit in auditLogs)
        {
            if (audit.User.IsBot) return;
            
            await Log($"> {audit.User.Username} ({audit.User.Id}) a supprimé un fil dans le salon {threadChannel.ParentChannel.Name} de {threadChannel.Guild.Name}.");
            await Log($"  * Createur: {threadChannel.Owner.Username}");
            await Log($"  * Serveur: {threadChannel.Guild.Name}");
            await Log($"  * Salon: {threadChannel.ParentChannel.Name}");
            await Log($"  * Nom: {threadChannel.Name}");
            await Log($"  * NSFW: {threadChannel.IsNsfw}");
            await Log($"  * Bloqué: {threadChannel.IsLocked}");
            await Log($"  * Privée: {threadChannel.IsPrivateThread}");
        }
    }

    private async Task OnThreadUpdated(Cacheable<SocketThreadChannel, ulong> cachedBefore, SocketThreadChannel after)
    {
        if (after == null) return;

        var before = await cachedBefore.GetOrDownloadAsync();
        if (before == null) return;

        var auditLogs = await after.Guild.GetAuditLogsAsync(1, actionType: ActionType.ThreadDelete).FlattenAsync();

        foreach (var audit in auditLogs)
        {
            if (audit.User.IsBot) return;
            
            if (after.Name != before.Name)
            {
                await Log($"> {audit.User.Username} ({audit.User.Id}) a modifié le nom du fil {before.Name} dans {before.Guild.Name}.");
                await Log($"  * Context: {before.Name} -> {after.Name}");
            }

            if (after.IsLocked != before.IsLocked)
            {
                await Log($"> {audit.User.Username} ({audit.User.Id}) a bloqué le fil {before.Name} dans {before.Guild.Name}.");
            }

            if (after.IsNsfw != before.IsNsfw)
            {
                await Log($"> {audit.User.Username} ({audit.User.Id}) a rendu NSFW le fil {before.Name} dans {before.Guild.Name}.");
            }

            if (after.IsPrivateThread != before.IsPrivateThread)
            {
                await Log($"> {audit.User.Username} ({audit.User.Id}) a rendu privvé le fil {before.Name} dans {before.Guild.Name}.");
            }

            if (after.MemberCount != before.MemberCount)
            {
                if (before.MemberCount > after.MemberCount)
                {
                    await Log($"> Le nombre de personnes sur le fil {before.Name} a diminué.");
                    await Log($"  * Context: {before.MemberCount} -> {after.MemberCount}");
                }
                else if (before.MemberCount < after.MemberCount)
                {
                    await Log($"> Le nombre de personnes sur le fil {before.Name} a augmenté.");
                    await Log($"  * Context: {before.MemberCount} -> {after.MemberCount}");
                }
            }
        }
    }

    private async Task OnThreadUserJoined(SocketThreadUser threadUser)
    {
        if (threadUser == null) return;
        if (threadUser.IsBot) return;

        await Log($"> {threadUser.Username} a rejoins le fil {threadUser.Thread.Name} du le salon {threadUser.Thread.ParentChannel.Name} de {threadUser.Guild.Name}.");
    }

    private async Task OnThreadUserLeft(SocketThreadUser threadUser)
    {
        if (threadUser == null) return;
        if (threadUser.IsBot) return;

        await Log($"> {threadUser.Username} a quitté le fil {threadUser.Thread.Name} du le salon {threadUser.Thread.ParentChannel.Name} de {threadUser.Guild.Name}.");
    }

    private async Task OnGuildEventCreated(SocketGuildEvent guildEvent)
    {
        if (guildEvent == null) return;

        await Log($"> {guildEvent.Creator.Username} ({guildEvent.Creator.Id}) a commencé un nouvel événement.");
        await Log($"  * Createur: {guildEvent.Creator.Username} ({guildEvent.Creator.Id})");
        await Log($"  * Serveur: {guildEvent.Guild.Name}");
        await Log($"  * Salon: {guildEvent.Channel.Name}");
        await Log($"  * Nom: {guildEvent.Name}");
        await Log($"  * Description: {guildEvent.Description}");
        await Log($"  * Bannière: {guildEvent.GetCoverImageUrl()}");
        await Log($"  * Début: {guildEvent.StartTime.ToString()}");
    }

    private async Task OnGuildEventCompleted(SocketGuildEvent guildEvent)
    {
        await Log($"> L'événement {guildEvent.Name} c'est terminé.");
        await Log($"  * Createur: {guildEvent.Creator.Username} ({guildEvent.Creator.Id})");
        await Log($"  * Serveur: {guildEvent.Guild.Name}");
        await Log($"  * Salon: {guildEvent.Channel.Name}");
        await Log($"  * Nom: {guildEvent.Name}");
        await Log($"  * Description: {guildEvent.Description}");
        await Log($"  * Bannière: {guildEvent.GetCoverImageUrl()}");
        await Log($"  * Début: {guildEvent.StartTime.ToString()}");
        await Log($"  * Fin: {guildEvent.EndTime.ToString()}");
        await Log($"  * Utilisateurs: {guildEvent.UserCount}");
    }

    private async Task OnGuildEventCancelled(SocketGuildEvent guildEvent)
    {
        if (guildEvent == null) return;

        // 100 event_created
        // 101 event_updated
        // 102 event_deleted

        var auditLogs = await guildEvent.Guild.GetAuditLogsAsync(1, actionType: (ActionType)102).FlattenAsync();

        foreach (var audit in auditLogs)
        {
            await Log($"> {audit.User.Username} ({audit.User.Id}) a annulé l'événement {guildEvent.Name}.");
        }
    }

    private async Task OnGuildEventStarted(SocketGuildEvent guildEvent)
    {
        if (guildEvent == null) return;

        await Log($"> L'événement {guildEvent.Name} a commencé.");
    }

    private async Task OnGuildEventUpdated(Cacheable<SocketGuildEvent, ulong> cachedBefore, SocketGuildEvent after)
    {
        if (after == null) return;

        var before = await cachedBefore.GetOrDownloadAsync();
        if (before == null) return;

        // 100 event_created
        // 101 event_updated
        // 102 event_deleted

        var auditLogs = await after.Guild.GetAuditLogsAsync(1, actionType: (ActionType)101).FlattenAsync();

        foreach (var audit in auditLogs)
        {
            if (after.Name != before.Name)
            {
                await Log($"> {audit.User.Username} ({audit.User.Id}) a modifié le nom de l'événement {before.Name}.");
                await Log($"  * Context: {before.Name} -> {after.Name}");
            }

            if (after.Description != before.Description)
            {
                await Log($"> {audit.User.Username} ({audit.User.Id}) a modifié la description de l'événement {before.Name}.");
                await Log($"  * Avant: {before.Description}");
                await Log($"  * Après: {after.Description}");
            }

            if (after.CoverImageId != before.CoverImageId)
            {
                await Log($"> {audit.User.Username} ({audit.User.Id}) a modifié la bannière de l'événement {before.Name}.");
                await Log($"  * Context: {before.GetCoverImageUrl()} -> {after.GetCoverImageUrl()}");
            }

            if (after.StartTime != before.StartTime)
            {
                await Log($"> {audit.User.Username} ({audit.User.Id}) a modifié la date de commencement de l'événement {before.Name}.");
                await Log($"  * Context: {before.StartTime.ToString()} -> {after.StartTime.ToString()}");
            }

            if (after.Channel.Id != before.Channel.Id)
            {
                await Log($"> {audit.User.Username} ({audit.User.Id}) a modifié le salon de l'événement {before.Name}.");
                await Log($"  * Context: {before.Channel.Name} -> {after.Channel.Name}");
            }
        }
    }

    private async Task OnGuildEventUserAdded(Cacheable<SocketUser, RestUser, IUser, ulong> cachedUser, SocketGuildEvent guildEvent)
    {
        if (guildEvent == null) return;

        var user = await cachedUser.GetOrDownloadAsync();
        if (user == null) return;
        if (user.IsBot) return;

        await Log($"> {user.Username} ({user.Id}) a était ajouté à l'événement {guildEvent.Name}.");
    }

    private async Task OnGuildEventUserRemoved(Cacheable<SocketUser, RestUser, IUser, ulong> cachedUser, SocketGuildEvent guildEvent)
    {
        if (guildEvent == null) return;

        var user = await cachedUser.GetOrDownloadAsync();
        if (user == null) return;
        if (user.IsBot) return;

        await Log($"> {user.Username} ({user.Id}) a était enlevé de l'événement {guildEvent.Name}.");
    }

    private async Task OnGuildStickerCreated(SocketCustomSticker sticker)
    {
        if (sticker == null) return;

        await Log($"> {sticker.Author.Username} ({sticker.Author.Id}) a ajouté un nouveau sticker dans {sticker.Guild.Name}.");
        await Log($"  * Createur: {sticker.Author.Username} ({sticker.Author.Username})");
        await Log($"  * Serveur: {sticker.Guild.Name}");
        await Log($"  * Nom: {sticker.Name}");
        await Log($"  * Description: {sticker.Description}");
    }

    private async Task OnGuildStickerDeleted(SocketCustomSticker sticker)
    {
        if (sticker == null) return;

        var auditLogs = await sticker.Guild.GetAuditLogsAsync(1, actionType: ActionType.StickerDeleted).FlattenAsync();

        foreach (var audit in auditLogs)
        {
            await Log($"> {audit.User.Username} ({audit.User.Id}) a supprimé le sticker {sticker.Name} de {sticker.Guild.Name}.");
            await Log($"  * Createur: {sticker.Author.Username} ({sticker.Author.Username})");
            await Log($"  * Serveur: {sticker.Guild.Name}");
            await Log($"  * Nom: {sticker.Name}");
            await Log($"  * Description: {sticker.Description}");
        }
    }

    private async Task OnGuildStickerUpdated(SocketCustomSticker before, SocketCustomSticker after)
    {
        if (before == null) return;
        if (after == null) return;

        var auditLogs = await after.Guild.GetAuditLogsAsync(1, actionType: ActionType.StickerDeleted).FlattenAsync();

        foreach (var audit in auditLogs)
        {
            if (after.Name != before.Name)
            {
                await Log($"> {audit.User.Username} ({audit.User.Id}) a modifié le nom du sticker {before.Name} de {before.Guild.Name}.");
                await Log($"  * Context: {before.Name} -> {after.Name}");
            }

            if (after.Description != before.Description)
            {
                await Log($"> {audit.User.Username} ({audit.User.Id}) a modifié la description du sticker {before.Name} de {before.Guild.Name}.");
                await Log($"  * Avant: {before.Description}");
                await Log($"  * Après: {after.Description}");
            }
        }
    }

    private async Task OnButtonExecuted(SocketMessageComponent component)
    {
        if (component == null) return;

        if (component.Data.CustomId == "rules")
        {
            if (component.User.IsBot) return;

            await Log($"> {component.User.Username} ({component.User.Id}) a accepté le règlement.");

            if (component.User is SocketGuildUser guildUser)
            {
                await guildUser.AddRoleAsync(501845907151519745);
            }

            var embed = new EmbedBuilder();
            embed.Title = "C'était la bonne décision. Crois-moi.";
            embed.ImageUrl = "https://cdn.discordapp.com/attachments/437191464305295375/986224921946509332/unknown.png";
            embed.Color = new Color(0x2f3136);

            await component.User.SendMessageAsync(embed: embed.Build());
        }
    }

    /* ---------------------------------- */

    [SlashCommand("clear", "Efface un nombre de message choisit.")]
    [Discord.Interactions.RequireUserPermission(GuildPermission.ManageMessages)]
    public async Task Clear([Discord.Interactions.Summary("nombre", "Nombre de message à effacer.")] int number)
    {
        isClearing = true;
        if (number > 100) number = 99;
        if (number <= 0) number = 0;

        var messages = await Context.Channel.GetMessagesAsync(number).FlattenAsync();
        var filteredMessages = messages.Where(x => (DateTimeOffset.UtcNow - x.Timestamp).TotalDays <= 14);
        int count = filteredMessages.Count();

        if (count >= 0 && Context.Channel is ITextChannel channel)
        {
            await channel.DeleteMessagesAsync(filteredMessages);
        }

        await Log($"> {Context.User.Username} ({Context.User.Id}) a utilisé la commande 'clear' et a effacé {count} message(s).");

        var embed = new EmbedBuilder();
        embed.Title = $"{count} message(s) effacé.";
        embed.Color = new Color(0x2f3136);

        await RespondAsync(embed: embed.Build(), ephemeral: true);
        isClearing = false;
    }

    [SlashCommand("link", "Donne un lien d'invitation.")]
    [EnabledInDm(true)]
    public async Task Link([Discord.Interactions.Summary("serveur", "Le serveur choisit.")] Serveur server = Serveur.Principal)
    {
        if (server == Serveur.Principal)
        {
            await Log($"> {Context.User.Username} ({Context.User.Id}) a utilisé la commande 'link principal'.");

            var embed = new EmbedBuilder();
            embed.Title = "https://discord.gg/B4hzjSrVCJ";
            embed.Url = "https://discord.gg/B4hzjSrVCJ";
            embed.Color = new Color(0x2f3136);

            await RespondAsync(embed: embed.Build(), ephemeral: true);
        }
        else if (server == Serveur.Staff)
        {
            await Log($"> {Context.User.Username} ({Context.User.Id}) a utilisé la commande 'link staff'.");

            var embed = new EmbedBuilder();
            embed.Title = "https://discord.gg/dpFb93r";
            embed.Url = "https://discord.gg/dpFb93r";
            embed.Color = new Color(0x2f3136);

            await RespondAsync(embed: embed.Build(), ephemeral: true);
        }
    }

    [SlashCommand("stop", "Permet d'éteindre le bot.")]
    [Discord.Interactions.RequireUserPermission(GuildPermission.Administrator)]
    [EnabledInDm(true)]
    public async Task Stop()
    {
        await Log($"> {Context.User.Username} ({Context.User.Id}) a arrêté le bot.");

        var embed = new EmbedBuilder();
        embed.Title = "Ambition s'est éteint.";
        embed.Color = new Color(0x2f3136);

        await RespondAsync(embed: embed.Build(), ephemeral: true);

        await client.StopAsync();
        Environment.Exit(0);
    }

    [SlashCommand("restart", "Permet de redémarrer le bot.")]
    [Discord.Interactions.RequireUserPermission(GuildPermission.Administrator)]
    [EnabledInDm(true)]
    public async Task Restart()
    {
        await Log($"> {Context.User.Username} ({Context.User.Id}) a redémarré le bot.");

        var embed = new EmbedBuilder();
        embed.Title = "Ambition va redémarré.";
        embed.Color = new Color(0x2f3136);

        await RespondAsync(embed: embed.Build(), ephemeral: true);

        await client.StopAsync();
        new Bot().RunBot().GetAwaiter().GetResult();
    }

    [SlashCommand("avatar", "Donne l'image d'une personne.")]
    [EnabledInDm(true)]
    public async Task Avatar([Discord.Interactions.Summary("utilisateur", "La personne choisit. Cela peut être sont identifiant.")] IUser user = null)
    {
        if (user != null)
        {
            await Log($"> {Context.User.Username} ({Context.User.Id}) a utilisé la commande 'avatar' sur {user.Username} ({user.Id}).");

            var embed = new EmbedBuilder();
            embed.Title = "Lien direct";
            embed.Url = user.GetAvatarUrl();
            embed.ImageUrl = user.GetAvatarUrl();
            embed.Color = new Color(0x2f3136);

            await RespondAsync(embed: embed.Build(), ephemeral: true);
        }
        else
        {
            await Log($"> {Context.User.Username} ({Context.User.Id}) a utilisé la commande 'avatar'.");

            var embed = new EmbedBuilder();
            embed.Title = "Lien direct";
            embed.Url = Context.User.GetAvatarUrl();
            embed.ImageUrl = Context.User.GetAvatarUrl();
            embed.Color = new Color(0x2f3136);

            await RespondAsync(embed: embed.Build(), ephemeral: true);
        }
    }

    [SlashCommand("icon", "Donne l'icon du serveur.")]
    [EnabledInDm(true)]
    public async Task Icon([Discord.Interactions.Summary("serveur", "Le serveur choisit.")] Serveur server = Serveur.Principal)
    {
        if (server == Serveur.Principal)
        {
            await Log($"> {Context.User.Username} ({Context.User.Id}) a utilisé la commande 'icon principal'");

            var embed = new EmbedBuilder();
            embed.Title = "Lien direct";
            embed.Url = Context.Client.GetGuildAsync(501824700486516766).Result.IconUrl;
            embed.ImageUrl = Context.Client.GetGuildAsync(501824700486516766).Result.IconUrl;
            embed.Color = new Color(0x2f3136);

            await RespondAsync(embed: embed.Build(), ephemeral: true);
        }
        else if (server == Serveur.Staff)
        {
            await Log($"> {Context.User.Username} ({Context.User.Id}) a utilisé la commande 'icon staff'");

            var embed = new EmbedBuilder();
            embed.Title = "Lien direct";
            embed.Url = Context.Client.GetGuildAsync(583414864677175306).Result.IconUrl;
            embed.ImageUrl = Context.Client.GetGuildAsync(583414864677175306).Result.IconUrl;
            embed.Color = new Color(0x2f3136);

            await RespondAsync(embed: embed.Build(), ephemeral: true);
        }
    }

    [SlashCommand("raidmode", "Permet de metter le bot en mode raid.")]
    [Discord.Interactions.RequireUserPermission(GuildPermission.Administrator)]
    [EnabledInDm(true)]
    public async Task RaidMode([Discord.Interactions.Summary("etat", "Allumé (true) ou éteint (false).")] bool state)
    {
        if (state)
        {
            await Log($"> {Context.User.Username} ({Context.User.Id}) a activé le mode raid.");

            raidMode = true;

            var embed = new EmbedBuilder();
            embed.Title = "Mode raid activé.";
            embed.Color = new Color(0x00ff00);

            await RespondAsync(embed: embed.Build(), ephemeral: true);
        }
        else
        {
            await Log($"> {Context.User.Username} ({Context.User.Id}) a désactivé le mode raid.");

            raidMode = false;

            var embed = new EmbedBuilder();
            embed.Title = "Mode raid désactivé.";
            embed.Color = new Color(0xff0000);

            await RespondAsync(embed: embed.Build(), ephemeral: true);
        }
    }

    [SlashCommand("info", "Permet d'obtenir les informations d'une personne.")]
    [Discord.Interactions.RequireUserPermission(GuildPermission.ModerateMembers)]
    [EnabledInDm(true)]
    public async Task Info([Discord.Interactions.Summary("utilisateur", "La personne choisit. Cela peut être sont identifiant.")] IUser user)
    {
        var embed = new EmbedBuilder();

        if (user is SocketGuildUser guildUser)
        {
            embed.Title = $"{guildUser.Username}#{guildUser.Discriminator}";
            embed.Description += $"**Nom:** {guildUser.Username}\n";
            embed.Description += $"**Mention:** {guildUser.Mention}\n";
            embed.Description += $"**Pseudo:** {guildUser.Nickname}\n";
            embed.Description += $"\n";
            embed.Description += $"**Statut:** {guildUser.Status.ToString()}\n";
            embed.Description += $"**Identifiant:** {guildUser.Id}\n";
            embed.Description += $"**Date de création:** {guildUser.CreatedAt.DateTime.ToString()}\n";
            embed.Description += $"**Date d'arrivé:** {guildUser.JoinedAt.GetValueOrDefault().DateTime.ToString()}\n";
            embed.Description += $"**Puissance:** {guildUser.Hierarchy}\n";
            embed.Description += $"\n";
            embed.Description += $"**Muet:** {guildUser.IsSelfMuted}\n";
            embed.Description += $"**Sourdine:** {guildUser.IsSelfDeafened}\n";
            embed.Description += $"**En stream:** {guildUser.IsStreaming}\n";
            embed.Description += $"**Partage d'écran:** {guildUser.IsVideoing}\n";
            embed.Description += $"**Premium:** {guildUser.PremiumSince.GetValueOrDefault().DateTime.ToString()}\n";
            embed.Description += $"\n";
            embed.Description += $"**Autre serveurs:**\n";
            foreach (var guild in guildUser.MutualGuilds) embed.Description += ($"  > {guild.Name}\n");
            embed.Description += $"**Activitée(s):**\n";
            foreach (var activity in guildUser.Activities) embed.Description += ($"  > {activity.Name}\n");
            embed.ThumbnailUrl = guildUser.GetAvatarUrl();
            embed.Color = new Color(0x2f3136);
        }
        else
        {
            embed.Title = $"{user.Username}#{user.Discriminator}";
            embed.Description += $"**Nom:** {user.Username}\n";
            embed.Description += $"**Mention:** {user.Mention}\n";
            embed.Description += $"**Statut:** {user.Status.ToString()}\n";
            embed.Description += $"**Identifiant:** {user.Id}\n";
            embed.Description += $"**Date de création:** {user.CreatedAt.DateTime.ToString()}\n";
            embed.Description += $"\n";
            embed.Description += $"**Activitée(s):**\n";
            foreach (var activity in user.Activities) embed.Description += ($"  > {activity.Name}\n");
            embed.ThumbnailUrl = user.GetAvatarUrl();
            embed.Color = new Color(0x2f3136);
        }

        await RespondAsync(embed: embed.Build(), ephemeral: true);
    }

    /* ---------------------------------- */

    private async Task UpdateAvatar(int time)
    {
        var images = Directory.EnumerateFiles("./Avatars");
        int lastValue = 0;

        while (true)
        {
            var random = new Random();
            int randomValue = random.Next(1, images.Count() + 1);

            if (lastValue != randomValue)
            {
                await Task.Delay(time * 1000);

                await client.CurrentUser.ModifyAsync(x => x.Avatar = new Image($"./Avatars/{randomValue}.png"));

                await Log("> L'avatar a était mit à jour.");

                lastValue = randomValue;
            }
        }
    }

    private async Task UpdateActivity(int time)
    {
        var type = ActivityType.Playing;

        while (true)
        {
            var random = new Random();
            int randomActivity = random.Next(0, 3);
            int randomText = random.Next(0, activityList[randomActivity].Length + 1);

            if (randomActivity == 1) type = ActivityType.Watching;
            else if (randomActivity == 2) type = ActivityType.Listening;

            await client.SetGameAsync(activityList[randomActivity][randomText], type: type);
            await Log("> L'activité a était mit à jour.");
            await Task.Delay(time * 1000);
        }
    }

    private async Task UpdateLog(int time)
    {
        while (true)
        {
            await Task.Delay(time * 1000);
            await Log("> Les logs ont étaient archivé.");

            var date = DateTime.Now.ToString(new CultureInfo("fr-FR"));

            if (await client.GetChannelAsync(861977019297693697) is SocketTextChannel logChannelPrincipal)
            {
                await logChannelPrincipal.SendFileAsync("./Log.txt", $"```[{date}]```");
            }

            if (await client.GetChannelAsync(986202497028849684) is SocketTextChannel logChannelStaff)
            {
                await logChannelStaff.SendFileAsync("./Log.txt", $"```[{date}]```");
            }

            File.Delete("./Log.txt");
        }
    }

    private async Task AddReactions(SocketMessage message)
    {
        string[] args = message.Content.ToLower().Split(' ');

        if (args.Contains("amogus"))
        {
            await message.AddReactionAsync(new Emoji("📮"));
        }
        else if (args.Contains("cheh"))
        {
            await message.AddReactionAsync(new Emoji("🌾"));
        }
        else if (args.Contains("bienvenue") || args.Contains("bvn") || args.Contains("welcome"))
        {
            await message.AddReactionAsync(new Emoji("👋"));
        }
        else if (args.Contains("juan") || args.Contains("cheval"))
        {
            await message.AddReactionAsync(new Emoji("🐎"));
        }
        else if (args.Contains("sanglier") || args.Contains("sus") || args.Contains("lechonk"))
        {
            await message.AddReactionAsync(new Emoji("🐗"));
        }
        else if (args.Contains("mari"))
        {
            await message.AddReactionAsync(new Emoji("👁️"));
        }
        else if (args.Contains("pingu") || args.Contains("noot"))
        {
            await message.AddReactionAsync(new Emoji("🐧"));
        }
        else if (args.Contains("cul"))
        {
            await message.AddReactionAsync(new Emoji("🍑"));
        }
        else if (args.Contains("doot"))
        {
            await message.AddReactionAsync(new Emoji("🎺"));
        }
        else if (args.Contains("pied") || args.Contains("feet"))
        {
            await message.AddReactionAsync(new Emoji("🦶"));
        }
        else if (args.Contains("monkey") || args.Contains("singe"))
        {
            await message.AddReactionAsync(new Emoji("🦍"));
            await message.AddReactionAsync(new Emoji("🦧"));
        }
        else if (args.Contains("mosquée") || args.Contains("mosque"))
        {
            await message.AddReactionAsync(new Emoji("🔥"));
        }
        else if (args.Contains("jesus"))
        {
            await message.AddReactionAsync(new Emoji("🧔‍♀️"));
        }
    }

    private async Task VerifyBadWords(SocketMessage message)
    {
        if (message.Author is SocketGuildUser author && author.GuildPermissions.Administrator) return;
        if (message.Author.IsBot) return;

        string[] words = message.Content.ToLower().Split(' ');
        bool containBadWords = false;

        foreach (string word in words)
        {
            if (bannedWords.Contains(word)) containBadWords = true;
        }

        if (containBadWords)
        {
            isClearing = true;
            await message.DeleteAsync();

            await Log($"> Le message de {message.Author.Username} a été effacé, car il possédait des caractères bannis.");

            var embed = new EmbedBuilder();
            embed.Title = "Alors, écoute moi bien...";
            embed.Description = "Ton droit de parole, il va partir au Brézil. Et si tu continue, c'est toi qui vas y aller !";
            embed.ImageUrl = "https://cdn.discordapp.com/attachments/645024434947620884/985572626170802186/unknown.png";
            embed.Color = new Color(0xff0000);

            await message.Author.SendMessageAsync(embed: embed.Build());
            isClearing = false;
        }
    }

    private async Task VerifyUsername(SocketGuildUser member)
    {
        if (member.GuildPermissions.Administrator) return;

        string[] nickname = member.Nickname.ToLower().Split(' ');
        string[] username = member.Username.ToLower().Split(' ');
        bool containBadWords = false;

        foreach (string word in bannedWords)
        {
            if (nickname.Contains(word)) containBadWords = true;
            if (username.Contains(word)) containBadWords = true;
        }

        if (containBadWords)
        {
            await member.ModifyAsync(x => x.Nickname = "Connard !");

            await Log($"> Le pseudo de {member.Username} a été effacé, car il possédait des caractères bannis.");

            var embed = new EmbedBuilder();
            embed.Title = "Alors, écoute moi bien...";
            embed.Description = "Ton pseudo, il va partir au Brézil. Et si tu continue, c'est toi qui vas y aller !";
            embed.ImageUrl = "https://cdn.discordapp.com/attachments/645024434947620884/985572626170802186/unknown.png";
            embed.Color = new Color(0xff0000);

            await member.SendMessageAsync(embed: embed.Build());
        }
    }

    /* ---------------------------------- */

    private static async Task Log(string message)
    {
        var date = DateTime.Now.ToString(new CultureInfo("fr-FR"));

        using (StreamWriter log = new StreamWriter("Log.txt", append: true))
        {
            await log.WriteLineAsync($"[{date}] {message}");
        }

        Console.WriteLine($"{message}");
    }

    /* ---------------------------------- */

    public static void Main() => new Bot().RunBot().GetAwaiter().GetResult();
}

public class InteractionHandler
{
    private readonly DiscordSocketClient client;
    private readonly InteractionService interactionService;

    /* ---------------------------------- */

    public InteractionHandler(DiscordSocketClient _client, InteractionService _interactionService)
    {
        client = _client;
        interactionService = _interactionService;
    }

    public async Task InitializeAsync()
    {
        client.Ready += OnReady;
        interactionService.Log += OnLog;

        await interactionService.AddModulesAsync(Assembly.GetEntryAssembly(), null);

        client.InteractionCreated += OnInteraction;
    }

    /* ---------------------------------- */

    private Task OnLog(LogMessage message)
    {
        Console.WriteLine(message.ToString());
        return Task.CompletedTask;
    }

    private async Task OnReady()
    {
        await interactionService.RegisterCommandsGloballyAsync(true);
    }

    private async Task OnInteraction(SocketInteraction interaction)
    {
        try
        {
            var context = new SocketInteractionContext(client, interaction);
            var result = await interactionService.ExecuteCommandAsync(context, null);

            if (!result.IsSuccess)
            {
                switch (result.Error)
                {
                    case InteractionCommandError.UnmetPrecondition:
                        break;
                    
                    default:
                        break;
                }
            }
        }
        catch
        {
            if (interaction.Type is InteractionType.ApplicationCommand)
            {
                await interaction.GetOriginalResponseAsync().ContinueWith(async (message) => await message.Result.DeleteAsync());
            }
        }
    }
}
