namespace SampleApp.Api;

public interface IBlogService
{
    IEnumerable<BlogDto> GetAll();
    BlogDto Get(int id);
    void Upsert(BlogDto blog);
    void Remove(int id);
    IEnumerable<PostDto> GetPosts(int id);
    PostDto GetPost(int id);
    void UpsertPost(int blogId, PostDto post);
    void RemovePost(int id);
}
