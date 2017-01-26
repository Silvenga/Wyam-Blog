﻿using System;

using Wyam.Common.Configuration;
using Wyam.Common.Execution;
using Wyam.Common.IO;
using Wyam.Common.Meta;
using Wyam.Core.Modules.Contents;
using Wyam.Core.Modules.Control;
using Wyam.Core.Modules.Extensibility;
using Wyam.Core.Modules.IO;
using Wyam.Core.Modules.Metadata;
using Wyam.Html;

namespace Wyam.SlightBlog
{
    public class SlightBlog : IRecipe
    {
        public void Apply(IEngine engine)
        {
            // Global metadata defaults
            engine.GlobalMetadata[MetaKeys.Title] = "My Blog";
            engine.GlobalMetadata[MetaKeys.MarkdownExtensions] = "advanced+bootstrap";
            engine.GlobalMetadata[MetaKeys.PostsPath] = new DirectoryPath("posts");
            engine.GlobalMetadata[MetaKeys.PagesPath] = new DirectoryPath("pages");

            engine.Pipelines.Add(PipelineKeys.Pages,
                new ReadFiles(ctx => $"{ctx.DirectoryPath(MetaKeys.PagesPath).FullPath}/*.md"),
                new FrontMatter(new Yaml.Yaml()),
                new Execute(ctx => new Markdown.Markdown().UseConfiguration(ctx.String(MetaKeys.MarkdownExtensions))),
                new Concat(
                    new ReadFiles(ctx => $"{ctx.DirectoryPath(MetaKeys.PagesPath).FullPath}/*.cshtml"),
                    new FrontMatter(new Yaml.Yaml())
                ),
                new Meta(Keys.RelativeFilePath, (doc, ctx) =>
                {
                    var slug = doc.Get(DocumentKeys.Slug, doc.FilePath(Keys.SourceFileName).FileNameWithoutExtension.FullPath);
                    return slug + ".html";
                })
            );

            engine.Pipelines.Add(PipelineKeys.Posts,
                new ReadFiles(ctx => $"{ctx.DirectoryPath(MetaKeys.PostsPath).FullPath}/*.md"),
                new FrontMatter(new Yaml.Yaml()),
                new Execute(ctx => new Markdown.Markdown().UseConfiguration(ctx.String(MetaKeys.MarkdownExtensions))),
                new Where((doc, ctx) =>
                {
                    if (!doc.ContainsKey(DocumentKeys.Published) || doc.Get(DocumentKeys.Published) == null)
                    {
                        Common.Tracing.Trace.Warning($"Skipping {doc.Source} due to not having {DocumentKeys.Published} metadata");
                        return false;
                    }
                    if (doc.Get<DateTime>(DocumentKeys.Published) > DateTime.Now)
                    {
                        Common.Tracing.Trace.Warning(
                            $"Skipping {doc.Source} due to having {DocumentKeys.Published} metadata of {doc.Get<DateTime>(DocumentKeys.Published)} in the future (current date and time is {DateTime.Now})");
                        return false;
                    }
                    return true;
                }),
                new Meta(Keys.RelativeFilePath, (doc, ctx) =>
                {
                    var slug = doc.Get(DocumentKeys.Slug, doc.FilePath(Keys.SourceFileName).FileNameWithoutExtension.FullPath);
                    return slug + ".html";
                }),
                new OrderBy((doc, ctx) => doc.Get<DateTime>(DocumentKeys.Published)).Descending()
            );

            engine.Pipelines.Add(PipelineKeys.Foundation,
                new ReadFiles(
                    ctx => $"{{!{ctx.DirectoryPath(MetaKeys.PostsPath).FullPath},!{ctx.DirectoryPath(MetaKeys.PagesPath).FullPath},**}}/{{*.cshtml,!_*}}"),
                new FrontMatter(new Yaml.Yaml())
            );

            engine.Pipelines.Add(PipelineKeys.RenderPosts,
                new Documents(PipelineKeys.Posts),
                new Razor.Razor()
                    .WithLayout("/_PostLayout.cshtml"),
                new WriteFiles(".html"),
                new OrderBy((doc, ctx) => doc.Get<DateTime>(DocumentKeys.Published)).Descending()
            );

            engine.Pipelines.Add(PipelineKeys.RenderPages,
                new Documents(PipelineKeys.Pages),
                new Razor.Razor()
                    .WithLayout("/_PageLayout.cshtml"),
                new WriteFiles(".html")
            );

            engine.Pipelines.Add(PipelineKeys.RenderFoundation,
                new Documents(PipelineKeys.Foundation),
                new Razor.Razor()
                    .WithLayout("/_Layout.cshtml"),
                new WriteFiles(".html")
            );

            engine.Pipelines.Add(PipelineKeys.Redirects,
                new Documents(PipelineKeys.RenderPages),
                new Concat(
                    new Documents(PipelineKeys.RenderPosts)
                ),
                new Execute(ctx =>
                {
                    var redirect = new Redirect()
                        .WithMetaRefreshPages();
                    return redirect;
                }),
                new WriteFiles()
            );

            engine.Pipelines.Add(PipelineKeys.Resources,
                new CopyFiles("**/*{!.cshtml,!.md,}")
            );

            engine.Pipelines.Add(PipelineKeys.ValidateLinks,
                new If(ctx => ctx.Get<bool>(MetaKeys.ValidateAbsoluteLinks) || ctx.Get<bool>(MetaKeys.ValidateRelativeLinks),
                    new Documents(PipelineKeys.RenderPages),
                    new Concat(
                        new Documents(PipelineKeys.RenderPosts)
                    ),
                    new Concat(
                        new Documents(PipelineKeys.Resources)
                    ),
                    new Where((doc, ctx) =>
                    {
                        var destinationPath = doc.FilePath(Keys.DestinationFilePath);
                        return destinationPath != null
                               && (destinationPath.Extension == ".html" || destinationPath.Extension == ".htm");
                    }),
                    new Execute(ctx =>
                        new ValidateLinks()
                            .ValidateAbsoluteLinks(ctx.Get<bool>(MetaKeys.ValidateAbsoluteLinks))
                            .ValidateRelativeLinks(ctx.Get<bool>(MetaKeys.ValidateRelativeLinks))
                            .AsError(ctx.Get<bool>(MetaKeys.ValidateLinksAsError)
                            )
                    )
                )
            );
        }

        public void Scaffold(IDirectory directory)
        {
            // Add info page
            directory.GetFile("about.md").WriteAllText(
                @"Title: About Me
---
I'm awesome!");

            // Add post page
            directory.GetFile("posts/first-post.md").WriteAllText(
                @"Title: First Post
Published: 1/1/2016
Tags: Introduction
---
This is my first post!");
        }
    }
}